using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Users.Managers
{
    using Abp.Domain.Services;
    using Core.OData;
    using Dto;
    using Extensions;
    using Portal.LicenseMonitoringSystem.Users.Entities;

    public  class GroupManager : DomainService, IGroupManager
    {
        private readonly PortalClient _portalClient;
        public GroupManager(PortalClient portalClient)
        {
            _portalClient = portalClient;
        }
        public void Add(LicenseGroupDto input, int tenantId)
        {
            var groupToAdd = ObjectMapper.Map<LicenseGroup>(input);
            groupToAdd.TenantId = tenantId;

            try
            {
                _portalClient.AddGroup(groupToAdd);
                _portalClient.SaveChanges();
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
                _portalClient.UpdateGroup(userToUpdate);
                _portalClient.SaveChanges();
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
                _portalClient.DeleteGroup(id);
                _portalClient.SaveChanges();
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
