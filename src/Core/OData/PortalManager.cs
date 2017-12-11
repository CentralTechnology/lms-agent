namespace LMS.OData
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using Abp.Configuration;
    using Actions;
    using Castle.Core.Logging;
    using CentraStage;
    using Common.Client;
    using Common.Constants;
    using Common.Extensions;
    using Common.Managers;
    using Configuration;
    using Microsoft.OData.Client;
    using Polly;
    using Portal.LicenseMonitoringSystem.Users.Entities;
    using Portal.LicenseMonitoringSystem.Veeam.Entities;
    using SharpRaven;
    using SharpRaven.Data;
    using Tools;
    using Users.Compare;
    using Users.Extensions;
    using Users.Models;

    public class PortalManager : LMSManagerBase, IPortalManager
    {
        private readonly PortalWebApiClient _portalWebApiClient;
        private readonly LicenseUserCompareLogic _licenseUserCompareLogic = new LicenseUserCompareLogic();
        private readonly LicenseGroupCompareLogic _licenseGroupCompareLogic = new LicenseGroupCompareLogic();

        public Container Container { get; set; }

        protected Policy DefaultPolicy;

        public PortalManager(PortalWebApiClient portalWebApiClient)
        {
            Container = new Container(new Uri(Constants.DefaultServiceUrl))
            {
                IgnoreResourceNotFoundException = true,
                MergeOption = MergeOption.NoTracking
            };


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
                .Or<TaskCanceledException>()
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

        public void Detach(object entity)
        {
            Container.Detach(entity);
        }

        public void AddGroup(LicenseGroup licenseGroup) => Container.AddToLicenseGroups(licenseGroup);

        /// <summary>
        /// Add License User to License Group. License Group should be attached to the container before calling this method.
        /// </summary>
        /// <param name="licenseUser"></param>
        /// <param name="licenseGroup"></param>
        public void AddGroupToUser(LicenseUser licenseUser, LicenseGroup licenseGroup)
        {
            Container.AttachTo("LicenseUsers", licenseUser);
            Container.AddLink(licenseUser, "Groups", licenseGroup);
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
            e.Headers.Add("AccountId", SettingManager.GetSettingValue(AppSettingNames.AutotaskAccountId));
            e.Headers.Add("XSRF-TOKEN", _portalWebApiClient.GetAntiForgeryToken());
            e.Headers.Add("Authorization", $"Device {SettingManager.GetSettingValue(AppSettingNames.CentrastageDeviceId)}");
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
            Container.AttachTo("LicenseUsers", licenseUser);
            Container.DeleteLink(licenseUser, "Groups", licenseGroup);
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
                case TaskCanceledException taskCanceled:
                    Logger.Error(taskCanceled.Message);
                    Logger.Debug("Exception when querying the API.", taskCanceled);
                    break;
                default:
                    Logger.Error(ex.Message);
                    Logger.Debug(ex.ToString());
                    RavenClient.Capture(new SentryEvent(ex));
                    break;
            }
        }

        public List<LicenseGroupSummary> ListAllGroupIds()
        {
            return DefaultPolicy.Execute(() => Container.LicenseGroups
                .Select(g => new LicenseGroupSummary { Id = g.Id, Name = g.Name })
                .ToList());
        }

        public List<LicenseGroupSummary> ListAllGroupIds(Expression<Func<LicenseGroup, bool>> predicate)
        {
            return DefaultPolicy.Execute(() => Container.LicenseGroups
                .Where(predicate)
                .Select(g => new LicenseGroupSummary { Id = g.Id, Name = g.Name })
                .ToList());
        }

        public List<LicenseUserSummary> ListAllUserIds(Expression<Func<LicenseUser, bool>> predicate)
        {
            return DefaultPolicy.Execute(() => Container.LicenseUsers
                .Where(predicate)
                .Select(u => new LicenseUserSummary { Id = u.Id, DisplayName = u.DisplayName })
                .ToList());
        }

        public List<LicenseUserSummary> ListAllUserIds()
        {
            return DefaultPolicy.Execute(() => Container.LicenseUsers
                .Select(u => new LicenseUserSummary { Id = u.Id, DisplayName = u.DisplayName })
                .ToList());
        }

        public List<LicenseUserSummary> ListAllUserIdsByGroupId(Guid groupId)
        {
            return DefaultPolicy.Execute(() =>
                    Container.LicenseUsers
                        .Where(u => u.Groups.Any(g => g.Id == groupId))
                        .Select(u => new LicenseUserSummary { DisplayName = u.DisplayName, Id = u.Id }))
                .ToList();
        }

        public List<LicenseUser> ListAllUsersByGroupId(Guid groupId) => DefaultPolicy.Execute(() => Container.LicenseUsers.Where(u => u.Groups.Any(g => g.Id == groupId)).ToList());

        public LicenseGroup ListGroupById(Guid id) => DefaultPolicy.Execute(() => Container.LicenseGroups.Where(g => g.Id == id).SingleOrDefault());

        public ManagedSupport ListManagedSupportById(int id) => DefaultPolicy.Execute(() => Container.ManagedSupports.Where(ms => ms.Id == id).SingleOrDefault());

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
                DataServiceResponse serviceResponse = isBatch ? Container.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset) : Container.SaveChanges();
                ProcessResponse(serviceResponse);
            });
        }

        public bool UpdateGroup(LicenseGroup licenseGroup)
        {
            var existingGroup = Container.LicenseGroups.Where(lg => lg.Id == licenseGroup.Id).FirstOrDefault();
            if (existingGroup == null)
            {
                throw new NullReferenceException($"License Group {licenseGroup.Format(Logger.IsDebugEnabled)} cannot be found in the api.");
            }

            var result = _licenseGroupCompareLogic.Compare(existingGroup, licenseGroup);
            if (result.AreEqual)
            {
                Container.Detach(existingGroup);
                return false;
            }

            existingGroup.IsDeleted = false;
            existingGroup.Name = licenseGroup.Name;
            existingGroup.WhenCreated = licenseGroup.WhenCreated;

            Container.AttachTo("LicenseGroups", existingGroup);
            Container.UpdateObject(existingGroup);

            return true;
        }

        public void UpdateManagedSupport(ManagedSupport managedSupport)
        {
            var existingManagedSupport = Container.ManagedSupports.Where(ms => ms.Id == managedSupport.Id).FirstOrDefault();
            if (existingManagedSupport == null)
            {
                throw new NullReferenceException($"Managed Support {managedSupport.Id} cannot be found in the api.");
            }

            existingManagedSupport.CheckInTime = managedSupport.CheckInTime;
            existingManagedSupport.ClientVersion = managedSupport.ClientVersion;
            existingManagedSupport.Hostname = managedSupport.Hostname;
            existingManagedSupport.Status = managedSupport.Status;
            existingManagedSupport.UploadId = managedSupport.UploadId;

            Container.AttachTo("ManagedSupports", existingManagedSupport);
            Container.UpdateObject(existingManagedSupport);
        }

        public bool UpdateUser(LicenseUser licenseUser)
        {
            var existingUser = Container.LicenseUsers.Where(lu => lu.Id == licenseUser.Id).FirstOrDefault();
            if (existingUser == null)
            {
                throw new NullReferenceException($"License User {licenseUser.Format(Logger.IsDebugEnabled)} cannot be found in the api.");
            }

            var result = _licenseUserCompareLogic.Compare(existingUser, licenseUser);
            if (result.AreEqual)
            {
                // no changes so why make another call hmmm... ?
                Container.Detach(existingUser);
                return false;
            }

            existingUser.DisplayName = licenseUser.DisplayName;
            existingUser.Email = licenseUser.Email;
            existingUser.Enabled = licenseUser.Enabled;
            existingUser.FirstName = licenseUser.FirstName;
            existingUser.IsDeleted = false;
            existingUser.LastLoginDate = licenseUser.LastLoginDate;
            existingUser.SamAccountName = licenseUser.SamAccountName;
            existingUser.Surname = licenseUser.Surname;
            existingUser.WhenCreated = licenseUser.WhenCreated;

            Container.AttachTo("LicenseUsers", existingUser);
            Container.UpdateObject(existingUser);

            return true;
        }

        public void UpdateVeeam(Veeam veeam)
        {
            Container.AttachTo("Veeams", veeam);

            Container.UpdateObject(veeam);
            Container.SaveChanges();
        }
    }
}