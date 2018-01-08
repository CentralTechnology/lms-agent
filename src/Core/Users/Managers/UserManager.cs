namespace LMS.Users.Managers
{
    using System;
    using Abp.Domain.Services;
    using Common.Extensions;
    using Dto;
    using Extensions;
    using global::Hangfire.Server;
    using OData;
    using Portal.LicenseMonitoringSystem.Users.Entities;

    public class UserManager : DomainService, IUserManager
    {
        private readonly IPortalManager _portalManager;

        public UserManager(IPortalManager portalManager)
        {
            _portalManager = portalManager;
        }

        public void Add(PerformContext performContext, LicenseUserDto input, int managedSupportId, int tenantId)
        {
            var userToAdd = ObjectMapper.Map<LicenseUser>(input);
            userToAdd.ManagedSupportId = managedSupportId;
            userToAdd.TenantId = tenantId;

            try
            {
                _portalManager.AddUser(userToAdd);
                _portalManager.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.Error(performContext, $"An error occurred while creating {userToAdd.Format()}");
                Logger.Debug(performContext, "Exception when creating LicenseUser", ex);
                throw;
            }

            Logger.Info(performContext, $"+ {userToAdd.Format(Logger.IsDebugEnabled)}");
        }

        public void Update(PerformContext performContext, LicenseUserDto input)
        {
            var userToUpdate = ObjectMapper.Map<LicenseUser>(input);

            try
            {
                bool updated = _portalManager.UpdateUser(userToUpdate);
                _portalManager.SaveChanges();

                Logger.Info(performContext, $"{(updated ? "^" : "=")} {userToUpdate.Format(Logger.IsDebugEnabled)}");
            }
            catch (Exception ex)
            {
                Logger.Error(performContext, $"An error occurred while updating {userToUpdate.Format()}");
                Logger.Debug(performContext, "Exception when updateing LicenseUser", ex);
                throw;
            }
        }

        public void Delete(PerformContext performContext, Guid id)
        {
            try
            {
                _portalManager.DeleteUser(id);
                _portalManager.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.Error(performContext, $"An error occurred while deleting {id}");
                Logger.Debug(performContext, "Exception when deleting LicenseUser", ex);
                throw;
            }

            Logger.Info(performContext, $"- {id}");
        }
    }
}