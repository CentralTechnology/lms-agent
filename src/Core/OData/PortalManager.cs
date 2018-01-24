namespace LMS.OData
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using Abp.Configuration;
    using Actions;
    using Common.Constants;
    using Common.Extensions;
    using Common.Managers;
    using Configuration;
    using LicenseUserService;
    using Microsoft.OData.Client;
    using Polly;
    using Portal.LicenseMonitoringSystem.Users.Entities;
    using Portal.LicenseMonitoringSystem.Veeam.Entities;
    using SharpRaven.Data;
    using Tools;
    using Users.Compare;
    using Users.Extensions;
    using Users.Models;

    [SuppressMessage("ReSharper", "ReplaceWithSingleCallToFirstOrDefault")]
    [SuppressMessage("ReSharper", "ReplaceWithSingleCallToSingleOrDefault")]
    public class PortalManager : LMSManagerBase, IPortalManager
    {
        private readonly Policy _defaultPolicy;
        private readonly LicenseGroupCompareLogic _licenseGroupCompareLogic = new LicenseGroupCompareLogic();
        private readonly LicenseUserCompareLogic _licenseUserCompareLogic = new LicenseUserCompareLogic();

        public PortalManager()
        {
            Container = new Container(new Uri(Constants.DefaultServiceUrl))
            {
                IgnoreResourceNotFoundException = true,
                MergeOption = MergeOption.NoTracking
            };

            Container.BuildingRequest += Container_BuildingRequest;
            Container.SendingRequest2 += Container_SendingRequest2;
            Container.Timeout = 600;

            _defaultPolicy = Policy
                .Handle<DataServiceClientException>(e => e.StatusCode != 404 || e.StatusCode != 401)
                .Or<DataServiceRequestException>()
                .Or<DataServiceTransportException>()
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

        public Container Container { get; set; }

        public void Detach(object entity)
        {
            Container.Detach(entity);
        }

        public void AddGroup(LicenseGroup licenseGroup) => Container.AddToLicenseGroups(licenseGroup);

        /// <summary>
        ///     Add License User to License Group. License Group should be attached to the container before calling this method.
        /// </summary>
        /// <param name="licenseUser"></param>
        /// <param name="licenseGroup"></param>
        public void AddGroupToUser(LicenseUser licenseUser, LicenseGroup licenseGroup)
        {
            OperationResponse response = Container.LicenseUsers.ByKey(licenseUser.Id).AddUserToGroup(licenseGroup.Id).Execute();
            ProcessResponse(response);
        }

        public void AddManagedSupport(ManagedSupport managedSupport) => Container.AddToManagedSupports(managedSupport);

        public void AddUser(LicenseUser licenseUser) => Container.AddToLicenseUsers(licenseUser);

        public void AddVeeam(Veeam veeam)
        {
            Container.AddToVeeams(veeam);

            DataServiceResponse serviceResponse = Container.SaveChanges();
            ProcessResponse(serviceResponse);
        }

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
            OperationResponse response = Container.LicenseUsers.ByKey(licenseUser.Id).RemoveUserFromGroup(licenseGroup.Id).Execute();
            ProcessResponse(response);
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

        public int GenerateUploadId() => _defaultPolicy.Execute(() => Container.ManagedSupports.NewUploadId().GetValue());

        public int GetAccountIdByDeviceId(Guid deviceId) => _defaultPolicy.Execute(() => Container.Devices.GetAccountId(deviceId).GetValue());

        public int GetManagedSupportId(Guid deviceId) => _defaultPolicy.Execute(() => Container.ManagedSupports.GetUploadId(deviceId).GetValue());

        public List<LicenseGroupSummary> ListAllGroupIds()
        {
            return _defaultPolicy.Execute(() => Container.LicenseGroups
                .Select(g => new LicenseGroupSummary { Id = g.Id, Name = g.Name })
                .ToList());
        }

        public List<LicenseGroupSummary> ListAllGroupIds(Expression<Func<LicenseGroup, bool>> predicate)
        {
            return _defaultPolicy.Execute(() => Container.LicenseGroups
                .Where(predicate)
                .Select(g => new LicenseGroupSummary { Id = g.Id, Name = g.Name })
                .ToList());
        }

        public List<LicenseUserSummary> ListAllUserIds(Expression<Func<LicenseUser, bool>> predicate)
        {
            return _defaultPolicy.Execute(() => Container.LicenseUsers
                .Where(predicate)
                .Select(u => new LicenseUserSummary { Id = u.Id, DisplayName = u.DisplayName })
                .ToList());
        }

        public List<LicenseUserSummary> ListAllUserIds()
        {
            return _defaultPolicy.Execute(() => Container.LicenseUsers
                .Select(u => new LicenseUserSummary { Id = u.Id, DisplayName = u.DisplayName })
                .ToList());
        }

        public List<LicenseUserSummary> ListAllUserIdsByGroupId(Guid groupId)
        {
            return _defaultPolicy.Execute(() =>
                    Container.LicenseUsers
                        .Where(u => u.UserGroups.Any(g => g.GroupId == groupId))
                        .Select(u => new LicenseUserSummary { DisplayName = u.DisplayName, Id = u.Id }))
                .ToList();
        }

        public List<LicenseUser> ListAllUsersByGroupId(Guid groupId) => _defaultPolicy.Execute(() => Container.LicenseUsers.Where(u => u.UserGroups.Any(g => g.GroupId == groupId)).ToList());

        public LicenseGroup ListGroupById(Guid id) => _defaultPolicy.Execute(() => Container.LicenseGroups.Where(g => g.Id == id).SingleOrDefault());

        public ManagedSupport ListManagedSupportById(int id) => _defaultPolicy.Execute(() => Container.ManagedSupports.Where(ms => ms.Id == id).SingleOrDefault());

        public Veeam ListVeeamById(Guid id) => _defaultPolicy.Execute(() => Container.Veeams.Where(u => u.Id == id).SingleOrDefault());

        public void SaveChanges()
        {
            _defaultPolicy.Execute(() =>
            {
                DataServiceResponse serviceResponse = Container.SaveChanges();
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

        public bool UserExist(Guid userId)
        {
            return Container.LicenseUsers.Any(lu => lu.Id == userId);
        }

        private void Container_BuildingRequest(object sender, BuildingRequestEventArgs e)
        {
            e.Headers.Add("AccountId", SettingManager.GetSettingValue(AppSettingNames.AutotaskAccountId));
            e.Headers.Add("Authorization", $"Device {SettingManager.GetSettingValue(AppSettingNames.CentrastageDeviceId)}");
        }

        private void Container_SendingRequest2(object sender, SendingRequest2EventArgs e) => Logger.Debug($"{e.RequestMessage.Method} {e.RequestMessage.Url}");

        private void HandleRetryException(Exception ex)
        {
            switch (ex)
            {
                case DataServiceClientException dataServiceClient:
                    if (dataServiceClient.StatusCode == 404 || dataServiceClient.StatusCode == 401)
                    {
                        Logger.Error($"{dataServiceClient.StatusCode} - {dataServiceClient.Message}");
                        Logger.Debug(dataServiceClient.Message, dataServiceClient);
                        break;
                    }

                    Logger.Error(dataServiceClient.Message);
                    Logger.Debug(dataServiceClient.Message, dataServiceClient);
                    RavenClient.Capture(new SentryEvent(dataServiceClient));
                    break;
                case DataServiceQueryException dataServiceQuery:
                    Logger.Error(dataServiceQuery.Message);
                    Logger.Debug(dataServiceQuery.Message, dataServiceQuery);
                    break;
                case DataServiceRequestException dataServiceRequest:
                    Logger.Error(dataServiceRequest.Message);
                    Logger.Debug(dataServiceRequest.Message, dataServiceRequest);
                    break;
                case DataServiceTransportException dataServiceTransport:
                    Logger.Error(dataServiceTransport.Message);
                    Logger.Debug(dataServiceTransport.Message, dataServiceTransport);
                    break;
                case SocketException socket:
                    Logger.Error($"Portal api unavailable - {socket.Message}");
                    Logger.Debug(socket.Message, socket);
                    break;
                case IOException io:
                    Logger.Error($"Portal api unavailable - {io.Message}");
                    Logger.Debug(io.Message, io);
                    break;
                case WebException web:
                    Logger.Error($"Portal api unavailable - {web.Message}");
                    Logger.Debug(web.Message, web);
                    break;
                case TaskCanceledException taskCanceled:
                    Logger.Error(taskCanceled.Message);
                    Logger.Debug(taskCanceled.Message, taskCanceled);
                    break;
                default:
                    Logger.Error(ex.Message);
                    Logger.Debug(ex.Message, ex);
                    RavenClient.Capture(new SentryEvent(ex));
                    break;
            }
        }

        private void ProcessResponse(DataServiceResponse serviceResponse)
        {
            if (serviceResponse == null)
            {
                return;
            }

            foreach (OperationResponse operationResponse in serviceResponse)
            {
                ProcessResponse(operationResponse);
            }
        }

        private void ProcessResponse(OperationResponse operationResponse)
        {
            if (operationResponse == null)
            {
                return;
            }

            Logger.Debug($"Status Code: {operationResponse.StatusCode}");

            if (operationResponse.Error == null)
            {
                return;
            }

            Logger.Error($"Error: {operationResponse.Error.GetFullMessage()}");
            Logger.Debug(operationResponse.Error.ToString());
            throw operationResponse.Error;
        }
    }
}