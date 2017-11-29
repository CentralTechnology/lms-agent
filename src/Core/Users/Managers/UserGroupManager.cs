using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Users.Managers
{
    using Abp.Domain.Services;
    using Compare;
    using Core.OData;
    using Dto;
    using Extensions;
    using Portal.LicenseMonitoringSystem.Users.Entities;

    public class UserGroupManager : DomainService, IUserGroupManager
    {
        private readonly PortalClient _portalClient;
        public UserGroupManager(PortalClient portalClient)
        {
            _portalClient = portalClient;
        }

        public void AddUsersToGroup(LicenseGroupUsersDto groupMembers)
        {
            var group = _portalClient.ListGroupById(groupMembers.Id);
            _portalClient.Container.AttachTo("LicenseGroups", group);

            var remoteUsers = _portalClient.ListAllUsersByGroupId(groupMembers.Id);

            var newMembers = ObjectMapper.Map<List<LicenseUser>>(groupMembers.Users).Except(remoteUsers);

            foreach (var newMember in newMembers)
            {
                _portalClient.AddGroupToUser(newMember, group);
                _portalClient.SaveChanges();

                Logger.Info($"+ {newMember.Format(Logger.IsDebugEnabled)} has been added to {group.Format(Logger.IsDebugEnabled)}");
                _portalClient.Detach(newMember);

            }

            // need to detach the group 
            _portalClient.Detach(group);
        }
        public void DeleteUsersFromGroup(LicenseGroupUsersDto groupMembers)
        {
            var group = _portalClient.ListGroupById(groupMembers.Id);
            _portalClient.Container.AttachTo("LicenseGroups", group);

            var remoteUsers = _portalClient.ListAllUsersByGroupId(groupMembers.Id);

            var staleMembers = remoteUsers.Except(ObjectMapper.Map<List<LicenseUser>>(groupMembers.Users), new LicenseUserComparer());

            foreach (var staleMember in staleMembers)
            {
                _portalClient.DeleteGroupFromUser(staleMember, group);
                _portalClient.SaveChanges();

                Logger.Info($"+ {staleMember.Format(Logger.IsDebugEnabled)} has been removed from {group.Format(Logger.IsDebugEnabled)}");
                _portalClient.Detach(staleMember);
            }

            // need to detach the group
            _portalClient.Detach(group);
        }
    }
}
