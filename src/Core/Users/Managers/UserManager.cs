using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Users.Managers
{
    using Abp.Domain.Services;
    using Dto;
    using Extensions;
    using OData;
    using Portal.LicenseMonitoringSystem.Users.Entities;

    public class UserManager : DomainService, IUserManager
    {
        private readonly IPortalManager _portalManager;
        public UserManager(IPortalManager portalManager)
        {
            _portalManager = portalManager;
        }
        public void Add(LicenseUserDto input, int managedSupportId, int tenantId)
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
                Logger.Error($"An error occurred while creating {userToAdd.Format()}");
                Logger.Debug("Exception when creating LicenseUser", ex);
                throw;
            }

            Logger.Info($"+ {userToAdd.Format(Logger.IsDebugEnabled)}");
        }

        public void Update(LicenseUserDto input)
        {
            var userToUpdate = ObjectMapper.Map<LicenseUser>(input);

            try
            {
                _portalManager.UpdateUser(userToUpdate);
                _portalManager.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.Error($"An error occurred while updating {userToUpdate.Format()}");
                Logger.Debug("Exception when updateing LicenseUser", ex);
                throw;
            }

            Logger.Info($"^ {userToUpdate.Format(Logger.IsDebugEnabled)}");
        }

        public void Delete(Guid id)
        {
            try
            {
                _portalManager.DeleteUser(id);
                _portalManager.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.Error($"An error occurred while deleting {id}");
                Logger.Debug("Exception when deleting LicenseUser", ex);
                throw;
            }

            Logger.Info($"- {id}");
        }
    }
}
