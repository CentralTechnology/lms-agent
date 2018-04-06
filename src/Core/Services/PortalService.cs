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
    using Abp.Timing;
    using Abp.UI;
    using Authentication;
    using Common.Managers;
    using Default;
    using Microsoft.OData.Client;
    using Newtonsoft.Json;
    using Portal.LicenseMonitoringSystem.Users.Entities;
    using Portal.LicenseMonitoringSystem.Veeam.Entities;

    [SuppressMessage("ReSharper", "ReplaceWithSingleCallToFirstOrDefault")]
    [SuppressMessage("ReSharper", "ReplaceWithSingleCallToSingleOrDefault")]
    public class PortalService : LMSManagerBase, IPortalService, IShouldInitialize
    {
#if DEBUG
        private const string ServiceUri = "http://localhost:64755//odata";
#else
         private const string ServiceUri = "https://api-v2.portal.ct.co.uk/odata";
    #endif

        private readonly IPortalAuthenticationService _authService;
        private Container _context;

        public PortalService(IPortalAuthenticationService authService)
        {
            _authService = authService;
        }

        public DataServiceCollection<Veeam> GetVeeamServer()
        {
            var device = _authService.GetDevice();
            return new DataServiceCollection<Veeam>(_context.VeeamServers.Where(e => e.Id == device));
        }

        public DataServiceCollection<LicenseUser> GetUserById(Guid userId)
        {
            var account = _authService.GetAccount();
            return new DataServiceCollection<LicenseUser>(_context.Users.AddQueryOption("tenantId", account).Where(u => u.Id == userId));
        }

        public async Task UpdateVeeamServerAsync(Veeam update)
        {
            DataServiceCollection<Veeam> original = GetVeeamServer();
            if (original.Count != 1)
            {
                _context.AddToVeeamServers(update);
                await _context.SaveChangesAsync();

                return;
            }

            original[0].CheckInTime = Clock.Now;
            original[0].ClientVersion = update.ClientVersion;
            original[0].DeleterUserId = null;
            original[0].DeletionTime = null;
            original[0].Edition = update.Edition;
            original[0].ExpirationDate = update.ExpirationDate;
            original[0].Hostname = update.Hostname;
            original[0].HyperV = update.HyperV;
            original[0].IsDeleted = false;
            original[0].LicenseType = update.LicenseType;
            original[0].ProgramVersion = update.ProgramVersion;
            original[0].SupportId = update.SupportId;
            original[0].TenantId = update.TenantId;
            original[0].vSphere = update.vSphere;

            await _context.SaveChangesAsync();
        }

        public async Task AddUserAsync(LicenseUser user)
        {
            _context.AddToUsers(user);
            await _context.SaveChangesAsync();
        }

        public async Task AddGroupAsync(LicenseGroup group)
        {
            _context.AddToGroups(group);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(LicenseUser user)
        {
            _context.UpdateObject(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateGroupAsync(LicenseGroup group)
        {
            _context.UpdateObject(group);
            await _context.SaveChangesAsync();
        }

        public List<LicenseUserGroup> GetAllGroupUsers(Guid group)
        {
            var account = _authService.GetAccount();
            return _context.UserGroups.AddQueryOption("tenantId", account).Where(g => g.GroupId == group).ToList();
        }

        public async Task<List<LicenseUser>> GetAllUsersAsync()
        {
            var account = _authService.GetAccount();

            DataServiceCollection<LicenseUser> users = new DataServiceCollection<LicenseUser>(_context.Users.AddQueryOption("tenantId", account));

            while (users.Continuation != null)
            {
                users.Load(await _context.ExecuteAsync(users.Continuation));
            }

            return users.ToList();
        }

        public async Task<List<LicenseGroup>> GetAllGroupsAsync()
        {
            var account = _authService.GetAccount();

            DataServiceCollection<LicenseGroup> groups = new DataServiceCollection<LicenseGroup>(_context.Groups.AddQueryOption("tenantId", account));

            while (groups.Continuation != null)
            {
                groups.Load(await _context.ExecuteAsync(groups.Continuation));
            }

            return groups.ToList();
        }

        public async Task AddUserGroupAsync(LicenseUserGroup userGroup)
        {
            var account = _authService.GetAccount();

            _context.AddToUserGroups(userGroup);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserGroupAsync(LicenseUserGroup userGroup)
        {
            var account = _authService.GetAccount();

            _context.DeleteObject(userGroup);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(LicenseUser user)
        {
            _context.DeleteObject(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteGroupAsync(LicenseGroup group)
        {
            _context.DeleteObject(group);
            await _context.SaveChangesAsync();
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
            update.CheckInTime = DateTimeOffset.UtcNow;

            _context.UpdateObject(update);
            await _context.SaveChangesAsync();
        }

        public void Initialize()
        {
            ConfigureContainer();
        }

        private void ConfigureContainer()
        {
            Logger.Info("Configuring the api service.");

            _context = new Container(new Uri(ServiceUri));

            _context.ReceivingResponse += ContextReceivingResponse;
            _context.SendingRequest2 += ContextSendingRequest2;

            Logger.Info("Configuration complete!");
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

        private void ContextReceivingResponse(object sender, ReceivingResponseEventArgs e)
        {
            Logger.Debug($"Recieving response: {JsonConvert.SerializeObject(e.ResponseMessage, Formatting.Indented)}");

            if (!(e.ResponseMessage is HttpWebResponseMessage responseMessage))
            {
                throw new UserFriendlyException();
            }

            if (!(responseMessage.Response is HttpWebResponse response))
            {
                throw new UserFriendlyException();
            }

            if (response.StatusCode == HttpStatusCode.BadRequest ||
                response.StatusCode == HttpStatusCode.InternalServerError)
            {
                using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    var error = JsonConvert.DeserializeObject<ODataErrorResponse>(reader.ReadToEnd());

                    Logger.Error($"{response.StatusCode} ({(int) response.StatusCode}) - {error.Message}");

                    throw new UserFriendlyException((int) response.StatusCode, error.Message);
                }
            }
        }

        private void ContextSendingRequest2(object sender, SendingRequest2EventArgs e)
        {
            e.RequestMessage.SetHeader("Authorization", $"Bearer {_authService.GetToken()}");

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
    }

    public class ODataErrorResponse
    {
        [JsonProperty("@odata.context")]
        public string Context { get; set; }

        [JsonProperty("value")]
        public string Message { get; set; }
    }
}