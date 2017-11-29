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

    public  class GroupManager : DomainService, IGroupManager
    {
        private readonly IPortalManager _portalManager;
        public GroupManager(IPortalManager portalManager)
        {
            _portalManager = portalManager;
        }
        public void Add(LicenseGroupDto input, int tenantId)
        {
            var groupToAdd = ObjectMapper.Map<LicenseGroup>(input);
            groupToAdd.TenantId = tenantId;

            try
            {
                _portalManager.AddGroup(groupToAdd);
                _portalManager.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.Error($"An error occurred while creating {groupToAdd.Format()}");
                Logger.Debug("Exception when creating LicenseGroup", ex);
                throw;
            }

            Logger.Info($"+ {groupToAdd.Format(Logger.IsDebugEnabled)}");
        }

        public void Update(LicenseGroupDto input)
        {
            var userToUpdate = ObjectMapper.Map<LicenseGroup>(input);

            try
            {
                _portalManager.UpdateGroup(userToUpdate);
                _portalManager.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.Error($"An error occurred while updating {userToUpdate.Format()}");
                Logger.Debug("Exception when updateing LicenseGroup", ex);
                throw;
            }

            Logger.Info($"^ {userToUpdate.Format(Logger.IsDebugEnabled)}");
        }

        public void Delete(Guid id)
        {
            try
            {
                _portalManager.DeleteGroup(id);
                _portalManager.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.Error($"An error occurred while deleting {id}");
                Logger.Debug("Exception when deleting LicenseGroup", ex);
                throw;
            }

            Logger.Info($"- {id}");
        }
    }
}
