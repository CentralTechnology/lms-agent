namespace Core.OData
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using Abp.Dependency;
    using Abp.Logging;
    using Abp.Threading;
    using Actions;
    using Castle.Core.Logging;
    using Common.Constants;
    using Common.Extensions;
    using Common.Helpers;
    using LMS.Autotask;
    using LMS.CentraStage;
    using LMS.Common.Client;
    using LMS.Users.Models;
    using Microsoft.OData.Client;
    using Polly;
    using Portal.LicenseMonitoringSystem.Users.Entities;
    using Portal.LicenseMonitoringSystem.Veeam.Entities;
    using SharpRaven;
    using SharpRaven.Data;
    using Tools;

    public class PortalClient : ITransientDependency
    {
        public ILogger Logger { get; set; }
        private readonly ICentraStageManager _centraStageManager;
        private readonly IAutotaskManager _autotaskManager;
        private readonly PortalWebApiClient _portalWebApiClient;



       // protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly RavenClient RavenClient = Sentry.RavenClient.Instance;

        public Container Container = new Container(new Uri(Constants.DefaultServiceUrl))
        {
            IgnoreResourceNotFoundException = true,
            MergeOption = MergeOption.NoTracking
        };

        protected Policy DefaultPolicy;

        public PortalClient(ICentraStageManager centraStageManager, IAutotaskManager autotaskManager, PortalWebApiClient portalWebApiClient)
        {
            Logger = NullLogger.Instance;
            _centraStageManager = centraStageManager;
            _autotaskManager = autotaskManager;
            _portalWebApiClient = portalWebApiClient;

            Container.BuildingRequest += Container_BuildingRequest;
            Container.SendingRequest2 += Container_SendingRequest2;
            Container.Timeout = 600;

            DefaultPolicy = Policy
                .Handle<DataServiceClientException>(e => e.StatusCode != 404)
                .Or<DataServiceRequestException>()
                .Or<WebException>()
                .Or<SocketException>()
                .Or<IOException>()
                .WaitAndRetry(new[]
                {
                    TimeSpan.FromSeconds(15),
                    TimeSpan.FromSeconds(30),
                    TimeSpan.FromSeconds(60),
                    TimeSpan.FromSeconds(300),
                    TimeSpan.FromSeconds(600)
                }, (exception, timeSpan, retryCount, context) =>
                {
                    HandleRetryException(exception);

                    Logger.Error($"Retry {retryCount} of {context.PolicyKey} at {context.ExecutionKey}");
                });
        }

        public void AddGroup(LicenseGroup licenseGroup) => Container.AddToLicenseGroups(licenseGroup);

        public void AddGroupToUser(LicenseUser licenseUser, LicenseGroup licenseGroup)
        {
            Container.AddLink(licenseUser, "Groups", licenseGroup);
            Container.SaveChanges();
        }

        public void AddManagedSupport(ManagedSupport managedSupport) => Container.AddToManagedSupports(managedSupport);

        public void AddUser(LicenseUser licenseUser) => Container.AddToLicenseUsers(licenseUser);

        public void AddVeeam(Veeam veeam)
        {
            Container.AddToVeeams(veeam);
            
            DataServiceResponse serviceResponse = Container.SaveChanges();
            ProcessResponse(serviceResponse);
        }

        private void Container_BuildingRequest(object sender, BuildingRequestEventArgs e)
        {
            e.Headers.Add("AccountId", _autotaskManager.GetId().ToString());
            e.Headers.Add("XSRF-TOKEN", _portalWebApiClient.GetAntiForgeryToken());
            e.Headers.Add("Authorization", $"Device {_centraStageManager.GetId()}");
        }

        private void Container_SendingRequest2(object sender, SendingRequest2EventArgs e) => Logger.Debug($"{e.RequestMessage.Method} {e.RequestMessage.Url}");

        public void DeleteGroup(Guid id)
        {
            LicenseGroup group = Container.LicenseGroups.Where(g => g.Id == id).SingleOrDefault();
            if (group == null)
            {
                return;
            }

            Container.AttachTo("LicenseGroups", group);
            Container.DeleteObject(group);
            DataServiceResponse serviceResponse = Container.SaveChanges();
            ProcessResponse(serviceResponse);
        }

        public void DeleteGroupFromUser(LicenseUser licenseUser, LicenseGroup licenseGroup)
        {
            Container.DeleteLink(licenseUser, "Groups", licenseGroup);
            Container.SaveChanges();
        }

        public void DeleteUser(Guid id)
        {
            LicenseUser user = Container.LicenseUsers.Where(u => u.Id == id).SingleOrDefault();
            if (user == null)
            {
                return;
            }

            Container.AttachTo("LicenseUsers", user);
            Container.DeleteObject(user);
            DataServiceResponse serviceResponse = Container.SaveChanges();
            ProcessResponse(serviceResponse);
        }

        public int GenerateUploadId() => DefaultPolicy.Execute(() => Container.ManagedSupports.NewUploadId().GetValue());

        public int GetAccountIdByDeviceId(Guid deviceId) => DefaultPolicy.Execute(() => Container.Profiles.GetAccountId(deviceId).GetValue());

        public int GetManagedSupportId(Guid deviceId) => DefaultPolicy.Execute(() => Container.ManagedSupports.GetUploadId(deviceId).GetValue());

        protected void HandleRetryException(Exception ex)
        {
            switch (ex)
            {
                case DataServiceClientException dataServiceClient:
                    if (dataServiceClient.StatusCode == 404)
                    {
                        Logger.Error($"{dataServiceClient.StatusCode} - {dataServiceClient.Message}");
                        Logger.Debug(dataServiceClient.ToString());
                        break;
                    }

                    Logger.Error("Portal api unavailable.");
                    Logger.Debug(dataServiceClient.ToString());
                    RavenClient.Capture(new SentryEvent(ex));

                    break;
                case SocketException socket:
                    Logger.Error("Portal api unavailable.");
                    Logger.Debug(socket.ToString());
                    break;
                case IOException io:
                    Logger.Error("Portal api unavailable.");
                    Logger.Debug(io.ToString());
                    break;
                case WebException web:
                    Logger.Error("Portal api unavailable.");
                    Logger.Debug(web.ToString());
                    break;
                default:
                    Logger.Error(ex.Message);
                    Logger.Debug(ex.ToString());
                    RavenClient.Capture(new SentryEvent(ex));
                    break;
            }
        }

        public List<LicenseGroupSummary> ListAllActiveGroupIds()
        {
            return DefaultPolicy.Execute(() => Container.LicenseGroups
                .Where(g => !g.IsDeleted)
                .Select(g => new LicenseGroupSummary {Id = g.Id, Name = g.Name})
                .ToList());
        }

        public List<LicenseUserSummary> ListAllActiveUserIds()
        {
            return DefaultPolicy.Execute(() => Container.LicenseUsers
                .Where(u => !u.IsDeleted)
                .Select(u => new LicenseUserSummary {Id = u.Id, DisplayName = u.DisplayName})
                .ToList());
        }

        public List<LicenseGroup> ListAllGroups() => DefaultPolicy.Execute(() => Container.LicenseGroups.ToList());

        public List<LicenseUser> ListAllUsers() => DefaultPolicy.Execute(() => Container.LicenseUsers.ToList());

        public List<LicenseUser> ListAllUsersByGroupId(Guid groupId) => DefaultPolicy.Execute(() => Container.LicenseUsers.Where(u => u.Groups.Any(g => g.Id == groupId)).ToList());

        public LicenseGroup ListGroupById(Guid id) => DefaultPolicy.Execute(() => Container.LicenseGroups.Where(g => g.Id == id).SingleOrDefault());

        public ManagedSupport ListManagedSupportById(int id) => DefaultPolicy.Execute(() => Container.ManagedSupports.Where(ms => ms.Id == id).SingleOrDefault());

        public LicenseUser ListUserById(Guid id) => DefaultPolicy.Execute(() => Container.LicenseUsers.Where(u => u.Id == id).SingleOrDefault());

        public Veeam ListVeeamById(Guid id) => DefaultPolicy.Execute(() => Container.Veeams.Where(u => u.Id == id).SingleOrDefault());

        protected void ProcessResponse(DataServiceResponse serviceResponse)
        {
            if (serviceResponse == null)
            {
                return;
            }

            foreach (OperationResponse operationResponse in serviceResponse)
            {
                if (operationResponse == null)
                {
                    continue;
                }

                Logger.Debug($"Status Code: {operationResponse.StatusCode}");

                if (operationResponse.Error == null)
                {
                    continue;
                }

                Logger.Error($"Error: {operationResponse.Error.GetFullMessage()}");
                Logger.Debug(operationResponse.Error.ToString());
                throw operationResponse.Error;
            }
        }

        public void SaveChanges(bool isBatch = false)
        {
            DefaultPolicy.Execute(() =>
            {
                DataServiceResponse serviceResponse = isBatch ? Container.SaveChanges() : Container.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset);
                ProcessResponse(serviceResponse);
            });
        }

        public void UpdateGroup(LicenseGroup licenseGroup)
        {
            Container.AttachTo("LicenseGroups", licenseGroup);

            Container.UpdateObject(licenseGroup);
        }

        public void UpdateManagedSupport(ManagedSupport managedSupport)
        {
            Container.AttachTo("ManagedSupports", managedSupport);

            Container.UpdateObject(managedSupport);
        }

        public void UpdateUser(LicenseUser licenseUser)
        {
            Container.AttachTo("LicenseUsers", licenseUser);

            Container.UpdateObject(licenseUser);
        }

        public void UpdateVeeam(Veeam veeam)
        {
            Container.AttachTo("Veeams", veeam);

            Container.UpdateObject(veeam);
            Container.SaveChanges();
        }
    }
}