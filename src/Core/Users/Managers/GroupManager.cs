namespace LMS.Users.Managers
{
    using System;
    using Abp.Domain.Services;
    using Dto;
    using Extensions;
    using OData;
    using Portal.LicenseMonitoringSystem.Users.Entities;

    public class GroupManager : DomainService, IGroupManager
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
            var groupToUpdate = ObjectMapper.Map<LicenseGroup>(input);

            try
            {
                bool updated = _portalManager.UpdateGroup(groupToUpdate);
                _portalManager.SaveChanges();

                Logger.Info($"{(updated ? "^" : "=")} {groupToUpdate.Format(Logger.IsDebugEnabled)}");
            }
            catch (Exception ex)
            {
                Logger.Error($"An error occurred while updating {groupToUpdate.Format()}");
                Logger.Debug(ex.Message, ex);
                throw;
            }
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