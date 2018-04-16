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

    [SuppressMessage("ReSharper", "ReplaceWithSingleCallToFirstOrDefault")]
    [SuppressMessage("ReSharper", "ReplaceWithSingleCallToSingleOrDefault")]
    public class PortalService : LMSManagerBase, IPortalService, IShouldInitialize
    {
        private readonly IPortalAuthenticationService _authService;
        private Container _context;

        public PortalService(IPortalAuthenticationService authService)
        {
            _authService = authService;
        }

        public DataServiceCollection<Veeam> GetVeeamServerById(Guid id)
        {
            try
            {
                return new DataServiceCollection<Veeam>(_context.VeeamServers.ByKey(id));
            }
            catch (DataServiceQueryException ex) when (ex.Response.StatusCode == 404)
            {
                Logger.Debug(ex.Message, ex);
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
            var device = _authService.GetDevice();
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
                Logger.Debug($"Request Scheme: {e.RequestUri.Scheme}");
                if (e.RequestUri.Scheme != Uri.UriSchemeHttps)
                {
                    UriBuilder ub = new UriBuilder(e.RequestUri)
                    {
                        Scheme = Uri.UriSchemeHttps,
                        Port = 443
                    };
                    e.RequestUri = ub.Uri;
                    Logger.Debug($"Updated Uri: {e.RequestUri}");
                }
            }
        }

        private void ConfigureContainer()
        {
            Logger.Info("Configuring the api service.");

            _context = new Container(new Uri(GetServiceUri()));

            _context.BuildingRequest += _context_BuildingRequest;
            _context.ReceivingResponse += ContextReceivingResponse;
            _context.SendingRequest2 += ContextSendingRequest2;

            Logger.Debug($"Base Uri: {_context.BaseUri}");
            Logger.Info("Configuration complete!");
        }

        private void ContextReceivingResponse(object sender, ReceivingResponseEventArgs e)
        {
            Logger.Debug($"Recieving response: {JsonConvert.SerializeObject(e.ResponseMessage, Formatting.Indented)}");

            if (!(e.ResponseMessage is HttpWebResponseMessage responseMessage))
            {
                throw new UserFriendlyException($"Request Failed: {e.ResponseMessage.StatusCode}");
            }

            if (!(responseMessage.Response is HttpWebResponse response))
            {
                throw new UserFriendlyException($"Request Failed: {responseMessage.Response.StatusCode} {responseMessage.Response.StatusDescription}");
            }

            if (response.StatusCode == HttpStatusCode.BadRequest ||
                response.StatusCode == HttpStatusCode.InternalServerError ||
                response.StatusCode == HttpStatusCode.Unauthorized)
            {
                using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    Logger.Debug($"Content: {reader.ReadToEnd()}");

                    if (JsonValidationHelper.IsValidJson(reader.ReadToEnd()))
                    {
                        var error = JsonConvert.DeserializeObject<ODataErrorResponse>(reader.ReadToEnd());

                        Logger.Error($"{response.StatusCode} ({(int) response.StatusCode}) - {error.Message}");

                        throw new UserFriendlyException((int) response.StatusCode, error.Message);
                    }

                    throw new UserFriendlyException((int) response.StatusCode, reader.ReadToEnd());
                }
            }
        }

        private void ContextSendingRequest2(object sender, SendingRequest2EventArgs e)
        {
            e.RequestMessage.SetHeader("Authorization", $"Bearer {_authService.Token.access_token}");
            e.RequestMessage.SetHeader("Account", $"{_authService.GetAccount()}");

            if (e.RequestMessage is HttpWebRequestMessage message)
            {
                Logger.Debug($"Sending request: {message.Method} {message.Url}");
                Logger.Debug($"Sending request: {JsonConvert.SerializeObject(message, Formatting.Indented)}");
            }
            else
            {
                Logger.Debug($"Sending request: {e.RequestMessage.Method} {e.RequestMessage.Url}");
                Logger.Debug($"Sending request: {JsonConvert.SerializeObject(e.RequestMessage, Formatting.Indented)}");
            }
        }

        private string GetServiceUri()
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
                Logger.Debug(ex.Message, ex);
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