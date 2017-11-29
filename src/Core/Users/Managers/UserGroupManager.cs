using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Users.Managers
{
    using Abp.Domain.Services;
    using Compare;
    using Dto;
    using Extensions;
    using OData;
    using Portal.LicenseMonitoringSystem.Users.Entities;

    public class UserGroupManager : DomainService, IUserGroupManager
    {
        private readonly IPortalManager _portalManager;
        public UserGroupManager(IPortalManager portalManager)
        {
            _portalManager = portalManager;
        }

        public void AddUsersToGroup(LicenseGroupUsersDto groupMembers)
        {
            var group = _portalManager.ListGroupById(groupMembers.Id);
            _portalManager.Container.AttachTo("LicenseGroups", group);

            var remoteUsers = _portalManager.ListAllUsersByGroupId(groupMembers.Id);

            var newMembers = ObjectMapper.Map<List<LicenseUser>>(groupMembers.Users).Except(remoteUsers, new LicenseUserComparer());

            foreach (var newMember in newMembers)
            {
                _portalManager.AddGroupToUser(newMember, group);
                _portalManager.SaveChanges();

                Logger.Info($"+ {newMember.Format(Logger.IsDebugEnabled)} has been added to {group.Format(Logger.IsDebugEnabled)}");
                _portalManager.Detach(newMember);
            }

            // need to detach the group 
            _portalManager.Detach(group);
        }
        public void DeleteUsersFromGroup(LicenseGroupUsersDto groupMembers)
        {
            var group = _portalManager.ListGroupById(groupMembers.Id);
            _portalManager.Container.AttachTo("LicenseGroups", group);

            var remoteUsers = _portalManager.ListAllUsersByGroupId(groupMembers.Id);

            var staleMembers = remoteUsers.Except(ObjectMapper.Map<List<LicenseUser>>(groupMembers.Users), new LicenseUserComparer());

            foreach (var staleMember in staleMembers)
            {
                _portalManager.DeleteGroupFromUser(staleMember, group);
                _portalManager.SaveChanges();

                Logger.Info($"+ {staleMember.Format(Logger.IsDebugEnabled)} has been removed from {group.Format(Logger.IsDebugEnabled)}");
                _portalManager.Detach(staleMember);
            }

            // need to detach the group
            _portalManager.Detach(group);
        }
    }
}
