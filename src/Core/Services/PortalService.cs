namespace LMS.Core.Services
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using Abp;
    using Abp.Timing;
    using Authentication;
    using Common.Managers;
    using Default;
    using Microsoft.OData.Client;
    using Newtonsoft.Json;
    using Portal.LicenseMonitoringSystem.Veeam.Entities;

    [SuppressMessage("ReSharper", "ReplaceWithSingleCallToFirstOrDefault")]
    public class PortalService : LMSManagerBase, IPortalService, IShouldInitialize
    {
        private readonly IPortalAuthenticationService _authService;
        private Container _container;

        public PortalService(IPortalAuthenticationService authService)
        {
            _authService = authService;
        }

        public DataServiceCollection<Veeam> GetVeeamServer()
        {
            var device = _authService.GetDevice();
            return new DataServiceCollection<Veeam>(_container.VeeamServers.Where(e => e.Id == device));
        }

        public async Task UpdateVeeamServerAsync(Veeam update)
        {
            DataServiceCollection<Veeam> original = GetVeeamServer();
            if (original.Count != 1)
            {
                _container.AddToVeeamServers(update);
                await _container.SaveChangesAsync();

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

            await _container.SaveChangesAsync();
        }

        public void Initialize()
        {
            ConfigureContainer();
        }

        private void _container_ReceivingResponse(object sender, ReceivingResponseEventArgs e)
        {
            Logger.Debug($"Recieving response: {JsonConvert.SerializeObject(e.ResponseMessage, Formatting.Indented)}");

            // operation response or dataservice response
        }

        private void _container_SendingRequest2(object sender, SendingRequest2EventArgs e)
        {
            e.RequestMessage.SetHeader("Authorization", $"Bearer {_authService.GetToken()}");
            Logger.Debug($"Sending request: {e.RequestMessage.Method} {e.RequestMessage.Url}");
            Logger.Debug($"Sending request: {JsonConvert.SerializeObject(e.RequestMessage, Formatting.Indented)}");
        }

        private void ConfigureContainer()
        {
            Logger.Info("Configuring the api service.");

            _container = new Container(new Uri("https://api-v2.portal.ct.co.uk/odata"));

            _container.ReceivingResponse += _container_ReceivingResponse;
            _container.SendingRequest2 += _container_SendingRequest2;

            Logger.Info("Configuration complete!");
        }
    }
}