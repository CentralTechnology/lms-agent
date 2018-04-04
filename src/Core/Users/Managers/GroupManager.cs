namespace LMS.Users.Managers
{
    using System;
    using Abp.Domain.Services;
    using Common.Extensions;
    using Core.Common.Extensions;
    using Dto;
    using Extensions;
    using global::Hangfire.Server;
    using OData;
    using Portal.LicenseMonitoringSystem.Users.Entities;

    public class GroupManager : DomainService, IGroupManager
    {
        private readonly IPortalManager _portalManager;

        public GroupManager(IPortalManager portalManager)
        {
            _portalManager = portalManager;
        }

        public void Add(PerformContext performContext, LicenseGroupDto input, int tenantId)
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
                Logger.Error(performContext, $"An error occurred while creating {groupToAdd.Format()}");
                Logger.Debug(performContext, "Exception when creating LicenseGroup", ex);
                throw;
            }

            Logger.Info(performContext, $"+ {groupToAdd.Format(Logger.IsDebugEnabled)}");
        }

        public void Update(PerformContext performContext, LicenseGroupDto input)
        {
            var groupToUpdate = ObjectMapper.Map<LicenseGroup>(input);

            try
            {
                bool updated = _portalManager.UpdateGroup(groupToUpdate);
                _portalManager.SaveChanges();

                Logger.Info(performContext, $"{(updated ? "^" : "=")} {groupToUpdate.Format(Logger.IsDebugEnabled)}");
            }
            catch (Exception ex)
            {
                Logger.Error(performContext, $"An error occurred while updating {groupToUpdate.Format()}");
                Logger.Debug(performContext, ex.Message, ex);
                throw;
            }
        }

        public void Delete(PerformContext performContext, Guid id)
        {
            try
            {
                _portalManager.DeleteGroup(id);
                _portalManager.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.Error(performContext, $"An error occurred while deleting {id}");
                Logger.Debug(performContext, "Exception when deleting LicenseGroup", ex);
                throw;
            }

            Logger.Info(performContext, $"- {id}");
        }
    }
}