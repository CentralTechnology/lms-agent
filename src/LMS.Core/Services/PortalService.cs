namespace LMS.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using Abp;
    using Abp.Domain.Services;
    using Abp.UI;
    using Authentication;
    using Default;
    using Helpers;
    using Json;
    using Managers;
    using Microsoft.OData.Client;
    using Newtonsoft.Json;
    using Portal.LicenseMonitoringSystem.Users.Entities;
    using Portal.LicenseMonitoringSystem.Veeam.Entities;
    using Serilog;

    [SuppressMessage("ReSharper", "ReplaceWithSingleCallToFirstOrDefault")]
    [SuppressMessage("ReSharper", "ReplaceWithSingleCallToSingleOrDefault")]
    public class PortalService : DomainService, IPortalService, IShouldInitialize
    {
        private readonly ILogger _logger = Log.ForContext<PortalService>();
        private Container _context;

        public DataServiceCollection<Veeam> GetVeeamServerById(Guid id)
        {
            try
            {
                return new DataServiceCollection<Veeam>(_context.VeeamServers.ByKey(id));
            }
            catch (DataServiceQueryException ex) when (ex.Response.StatusCode == 404)
            {
                _logger.Debug(ex.Message, ex);
                return new DataServiceCollection<Veeam>();
            }
        }

        public DataServiceCollection<LicenseUser> GetUserById(Guid id)
        {
            return new DataServiceCollection<LicenseUser>(_context.Users.ByKey(id));
        }

        public async Task UpdateVeeamServerAsync(Veeam update)
        {
            _context.UpdateObject(update);
            await SaveChangesAsync();
        }

        public async Task AddUserAsync(LicenseUser user)
        {
            _context.AddToUsers(user);
            await SaveChangesAsync();
        }

        public async Task AddGroupAsync(LicenseGroup group)
        {
            _context.AddToGroups(group);
            await SaveChangesAsync();
        }

        public async Task UpdateUserAsync(LicenseUser user)
        {
            _context.UpdateObject(user);
            await SaveChangesAsync();
        }

        public async Task UpdateGroupAsync(LicenseGroup group)
        {
            _context.UpdateObject(group);
            await SaveChangesAsync();
        }

        public IEnumerable<LicenseUserGroup> GetAllGroupUsers(Guid group)
        {
            return _context.UserGroups.Where(ug => ug.GroupId == group);
        }

        public IEnumerable<LicenseUser> GetAllUsers()
        {
            return _context.Users;
        }

        public IEnumerable<LicenseGroup> GetAllGroups()
        {
            return _context.Groups;
        }

        public async Task AddUserGroupAsync(LicenseUserGroup userGroup)
        {
            _context.AddToUserGroups(userGroup);
            await SaveChangesAsync();
        }

        public async Task AddVeeamServerAsync(Veeam veeam)
        {
            _context.AddToVeeamServers(veeam);
            await SaveChangesAsync();
        }

        public async Task DeleteUserGroupAsync(LicenseUserGroup userGroup)
        {
            _context.DeleteObject(userGroup);
            await SaveChangesAsync();
        }

        public async Task DeleteUserAsync(LicenseUser user)
        {
            _context.DeleteObject(user);
            await SaveChangesAsync();
        }

        public async Task DeleteGroupAsync(LicenseGroup group)
        {
            _context.DeleteObject(group);
            await SaveChangesAsync();
        }

        public async Task AddManagedServerAsync(ManagedSupport managedSupport)
        {
            _context.AddToManagedServers(managedSupport);
            await SaveChangesAsync();
        }

        public ManagedSupport GetManagedServer()
        {
            var device = PortalAuthenticationService.Instance.GetDevice();
            return _context.ManagedServers.Where(e => e.DeviceId == device).SingleOrDefault();
        }

        public async Task UpdateManagedServerAsync(ManagedSupport update)
        {
            _context.UpdateObject(update);
            await SaveChangesAsync();
        }

        public void Initialize()
        {
            ConfigureContainer();
        }

        private void _context_BuildingRequest(object sender, BuildingRequestEventArgs e)
        {
            if (!DebuggingService.Debug)
            {
                _logger.Debug("Request Scheme: {Scheme}",e.RequestUri.Scheme);
                if (e.RequestUri.Scheme != Uri.UriSchemeHttps)
                {
                    UriBuilder ub = new UriBuilder(e.RequestUri)
                    {
                        Scheme = Uri.UriSchemeHttps,
                        Port = 443
                    };
                    e.RequestUri = ub.Uri;
                    _logger.Debug("Updated Uri: {RequestUri}",e.RequestUri);
                }
            }
        }

        private void ConfigureContainer()
        {
            _logger.Information("Configuring the api service.");

            _context = new Container(new Uri(GetServiceUri()));

            _context.BuildingRequest += _context_BuildingRequest;
            _context.ReceivingResponse += ContextReceivingResponse;
            _context.SendingRequest2 += ContextSendingRequest2;

            _logger.Debug("Base Uri: {BaseUri}",_context.BaseUri);
            _logger.Information("Configuration complete!");
        }

        private void ContextReceivingResponse(object sender, ReceivingResponseEventArgs e)
        {
            _logger.Debug("Receiving response: {@Response}", e.ResponseMessage);

            if (!(e.ResponseMessage is HttpWebResponseMessage responseMessage))
            {
                throw new UserFriendlyException($"Request Failed: {e.ResponseMessage.StatusCode}");
            }

            if (!(responseMessage.Response is HttpWebResponse response))
            {
                throw new UserFriendlyException($"Request Failed: {responseMessage.Response.StatusCode} {responseMessage.Response.StatusDescription}");
            }

            if (response.StatusCode == HttpStatusCode.GatewayTimeout)
            {
                using (var reader = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException(), Encoding.UTF8))
                {
                    _logger.Debug("{@Content}", reader.ReadToEnd());

                    throw new UserFriendlyException((int) response.StatusCode, "Web server received an invalid response while acting as a gateway or proxy server.");
                }
            }

            if (response.StatusCode == HttpStatusCode.BadRequest ||
                response.StatusCode == HttpStatusCode.InternalServerError ||
                response.StatusCode == HttpStatusCode.Unauthorized)
            {
                using (var reader = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException(), Encoding.UTF8))
                {
                    _logger.Debug("{@Content}", reader.ReadToEnd());

                    if (JsonValidationHelper.IsValidJson(reader.ReadToEnd()))
                    {
                        var error = JsonConvert.DeserializeObject<ODataErrorResponse>(reader.ReadToEnd());

                        _logger.Error("{StatusCodeText} ({StatusCode}) - {Error}", response.StatusCode, (int) response.StatusCode, error.Message);

                        throw new UserFriendlyException((int) response.StatusCode, error.Message);
                    }

                    throw new UserFriendlyException((int) response.StatusCode, reader.ReadToEnd());
                }
            }
        }

        private void ContextSendingRequest2(object sender, SendingRequest2EventArgs e)
        {
            e.RequestMessage.SetHeader("Authorization", $"Bearer {PortalAuthenticationService.Instance.GetAccessToken()}");
            e.RequestMessage.SetHeader("Account", $"{PortalAuthenticationService.Instance.GetAccount()}");
            try
            {
                if (e.RequestMessage is HttpWebRequestMessage message)
                {
                    _logger.Debug("Sending request: {Method} {Url}", message.Method, message.Url);
                    _logger.Debug("Sending request: {Message}", message);
                }
                else
                {
                    _logger.Debug("Sending request: {Method} {Url}", e.RequestMessage.Method, e.RequestMessage.Url);
                    _logger.Debug("Sending request: {Message}", e.RequestMessage);
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private static string GetServiceUri()
        {
            return DebuggingService.Debug ? "http://localhost:64755/odata" : "https://api-v2.portal.ct.co.uk/odata";
        }

        private Task SaveChangesAsync()
        {
            try
            {
                return _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Debug(ex.Message, ex);
                throw;
            }
        }
    }

    public class ODataErrorResponse
    {
        [JsonProperty("@odata.context")]
        public string Context { get; set; }

        [JsonProperty("value")]
        public string Message { get; set; }
    }
}