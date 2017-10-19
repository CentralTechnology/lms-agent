namespace Core.OData
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using Actions;
    using Common.Constants;
    using Common.Extensions;
    using Common.Helpers;
    using Microsoft.OData.Client;
    using Models;
    using NLog;
    using Polly;
    using Portal.LicenseMonitoringSystem.Users.Entities;
    using Portal.LicenseMonitoringSystem.Veeam.Entities;
    using SharpRaven;
    using SharpRaven.Data;
    using Tools;
    using Veeam.Models;

    [SuppressMessage("ReSharper", "ReplaceWithSingleCallToSingleOrDefault")]
    public class PortalClient
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected Container Container = new Container(new Uri(Constants.DefaultServiceUrl));

        private static readonly RavenClient RavenClient = Sentry.RavenClient.Instance;

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

        protected Policy DefaultPolicy;

        public PortalClient()
        {
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

        public void AddGroup(IEnumerable<LicenseGroup> licenseGroups)
        {
            foreach (LicenseGroup licenseGroup in licenseGroups)
            {
                Container.AddToLicenseGroups(licenseGroup);
            }

            DataServiceResponse serviceResponse = Container.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset);
            ProcessResponse(serviceResponse);
        }

        public void AddGroupToUser(LicenseUser licenseUser, LicenseGroup licenseGroup)
        {
            LicenseUser user = Container.LicenseUsers.Where(u => u.Id == licenseUser.Id).SingleOrDefault();
            if (user == null)
            {
                return;
            }

            Container.AddLink(user, "Groups", licenseGroup);
            DataServiceResponse serviceResponse = Container.SaveChanges();
            ProcessResponse(serviceResponse);
        }

        public void AddManagedSupport(ManagedSupport managedSupport)
        {
            Container.AddToManagedSupports(managedSupport);
            DataServiceResponse serviceResponse = Container.SaveChanges();
            ProcessResponse(serviceResponse);
        }

        public void AddUser(IEnumerable<LicenseUser> licenseUsers)
        {
            foreach (LicenseUser licenseUser in licenseUsers)
            {
                Container.AddToLicenseUsers(licenseUser);
            }

            DataServiceResponse serviceResponse = Container.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset);
            ProcessResponse(serviceResponse);
        }

        public void AddVeeam(Veeam veeam)
        {
            Container.AddToVeeams(veeam);

            DataServiceResponse serviceResponse = Container.SaveChanges();
            ProcessResponse(serviceResponse);
        }

        private void Container_BuildingRequest(object sender, BuildingRequestEventArgs e)
        {
            e.Headers.Add("AccountId", SettingManagerHelper.AccountId.ToString());
            e.Headers.Add("XSRF-TOKEN", SettingManagerHelper.Token);
            e.Headers.Add("Authorization", $"Device {SettingManagerHelper.DeviceId}");
        }

        private void Container_SendingRequest2(object sender, SendingRequest2EventArgs e)
        {
            Logger.Debug($"{e.RequestMessage.Method} {e.RequestMessage.Url}");
        }

        public void DeleteGroup(Guid id)
        {
            LicenseGroup group = Container.LicenseGroups.Where(g => g.Id == id).SingleOrDefault();
            if (group != null)
            {
                Container.DeleteObject(group);
                DataServiceResponse serviceResponse = Container.SaveChanges();
                ProcessResponse(serviceResponse);
            }
        }

        public void DeleteGroupFromUser(LicenseUser licenseUser, LicenseGroup licenseGroup)
        {
            LicenseUser user = Container.LicenseUsers.Where(u => u.Id == licenseUser.Id).SingleOrDefault();
            if (user == null)
            {
                return;
            }

            Container.DeleteLink(user, "Groups", licenseGroup);
            DataServiceResponse serviceResponse = Container.SaveChanges();
            ProcessResponse(serviceResponse);
        }

        public void DeleteUser(Guid id)
        {
            LicenseUser user = Container.LicenseUsers.Where(u => u.Id == id).SingleOrDefault();
            if (user != null)
            {
                Container.DeleteObject(user);
                DataServiceResponse serviceResponse = Container.SaveChanges();
                ProcessResponse(serviceResponse);
            }
        }

        public int GenerateUploadId()
        {
            return DefaultPolicy.Execute(() => Container.ManagedSupports.NewUploadId().GetValue());
        }

        public int GetAccountIdByDeviceId(Guid deviceId)
        {
            return DefaultPolicy.Execute(() => Container.Profiles.GetAccountId(deviceId).GetValue());
        }

        public int GetManagedSupportId(Guid deviceId)
        {
            return DefaultPolicy.Execute(() => Container.ManagedSupports.GetUploadId(deviceId).GetValue());
        }

        public List<Guid> ListAllActiveGroupIds()
        {
            return DefaultPolicy.Execute(() => Container.LicenseGroups
                .Where(g => !g.IsDeleted)
                .Select(g => new {g.Id})
                .AsEnumerable()
                .Select(g => g.Id)
                .ToList());
        }

        public List<Guid> ListAllActiveUserIds()
        {
            return DefaultPolicy.Execute(() => Container.LicenseUsers
                .Where(u => !u.IsDeleted)
                .Select(u => new {u.Id})
                .AsEnumerable()
                .Select(u => u.Id)
                .ToList());
        }

        public List<LicenseGroup> ListAllGroups()
        {
            return DefaultPolicy.Execute(() => Container.LicenseGroups.ToList());
        }

        public List<LicenseUser> ListAllUsers()
        {
            return DefaultPolicy.Execute(() => Container.LicenseUsers.ToList());
        }

        public List<LicenseUser> ListAllUsersByGroupId(Guid groupId)
        {
            return DefaultPolicy.Execute(() => Container.LicenseUsers.Where(u => u.Groups.Any(g => g.Id == groupId)).ToList());
        }

        public LicenseGroup ListGroupById(Guid id)
        {
            return DefaultPolicy.Execute(() => Container.LicenseGroups.Where(g => g.Id == id).SingleOrDefault());
        }

        public ManagedSupport ListManagedSupportById(int id)
        {
            return DefaultPolicy.Execute(() => Container.ManagedSupports.Where(ms => ms.Id == id).SingleOrDefault());
        }

        public LicenseUser ListUserById(Guid id)
        {
            return DefaultPolicy.Execute(() => Container.LicenseUsers.Where(u => u.Id == id).SingleOrDefault());
        }

        public Veeam ListVeeamById(Guid id)
        {
            return DefaultPolicy.Execute(() => Container.Veeams.Where(u => u.Id == id).SingleOrDefault());
        }

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

                if (operationResponse.Error != null)
                {
                    Logger.Error($"Error: {operationResponse.Error.GetFullMessage()}");
                    Logger.Debug(operationResponse.Error.ToString());
                    throw operationResponse.Error;
                }
            }
        }

        public void UpdateGroup(IEnumerable<LicenseGroupUpdateModel> updateModels)
        {
            foreach (LicenseGroupUpdateModel updateModel in updateModels)
            {
                LicenseGroup group = Container.LicenseGroups.Where(g => g.Id == updateModel.Id).SingleOrDefault();
                if (group != null)
                {
                    group.Name = updateModel.Name;
                    group.WhenCreated = updateModel.WhenCreated;

                    Container.UpdateObject(group);
                }
            }

            DefaultPolicy.Execute(() =>
            {
                DataServiceResponse serviceResponse = Container.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset);
                ProcessResponse(serviceResponse);
            });
        }

        public void UpdateManagedSupport(int id, ManagedSupportUpdateModel updateModel)
        {
            ManagedSupport managedSupport = Container.ManagedSupports.Where(ms => ms.Id == id).SingleOrDefault();
            if (managedSupport != null)
            {
                managedSupport.CheckInTime = updateModel.CheckInTime;
                managedSupport.ClientVersion = updateModel.ClientVersion;
                managedSupport.Hostname = updateModel.Hostname;
                managedSupport.Status = updateModel.Status;
                managedSupport.UploadId = updateModel.UploadId;

                Container.UpdateObject(managedSupport);
                DataServiceResponse serviceResponse = Container.SaveChanges();
                ProcessResponse(serviceResponse);
            }
        }

        public void UpdateUser(IEnumerable<LicenseUserUpdateModel> updateModels)
        {
            foreach (LicenseUserUpdateModel updateModel in updateModels)
            {
                LicenseUser user = Container.LicenseUsers.Where(u => u.Id == updateModel.Id).SingleOrDefault();
                if (user != null)
                {
                    user.DisplayName = updateModel.DisplayName;
                    user.Email = updateModel.Email;
                    user.Enabled = updateModel.Enabled;
                    user.FirstName = updateModel.FirstName;
                    user.LastLoginDate = updateModel.LastLoginDate;
                    user.SamAccountName = updateModel.SamAccountName;
                    user.Surname = updateModel.Surname;
                    user.WhenCreated = updateModel.WhenCreated;

                    Container.UpdateObject(user);
                }
            }

            DefaultPolicy.Execute(() =>
            {
                DataServiceResponse serviceResponse = Container.SaveChanges(SaveChangesOptions.BatchWithSingleChangeset);
                ProcessResponse(serviceResponse);
            });
        }

        public void UpdateVeeam(Guid id, VeeamUpdateModel updateModel)
        {
            Veeam veeam = Container.Veeams.Where(v => v.Id == id).SingleOrDefault();
            if (veeam != null)
            {
                veeam.CheckInTime = updateModel.CheckInTime;
                veeam.ClientVersion = updateModel.ClientVersion;
                veeam.Edition = updateModel.Edition;
                veeam.ExpirationDate = updateModel.ExpirationDate;
                veeam.HyperV = updateModel.HyperV;
                veeam.ProgramVersion = updateModel.ProgramVersion;
                veeam.Status = updateModel.Status;
                veeam.SupportId = updateModel.SupportId;
                veeam.vSphere = updateModel.vSphere;
            }
        }
    }
}