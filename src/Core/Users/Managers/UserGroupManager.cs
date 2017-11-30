namespace LMS.Users.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Abp.Domain.Services;
    using Compare;
    using Dto;
    using Extensions;
    using Models;
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
            LicenseGroup group = _portalManager.ListGroupById(groupMembers.Id);
            _portalManager.Container.AttachTo("LicenseGroups", group);

            var remoteUsers = _portalManager.ListAllUserIdsByGroupId(groupMembers.Id).ToDictionary(u => u.Id);

            var members = ObjectMapper.Map<List<LicenseUser>>(groupMembers.Users);

            foreach (var user in members)
            {
                bool userIsMember = remoteUsers.ContainsKey(user.Id);
                if (userIsMember)
                {
                    Logger.Info($"= {user.Format(Logger.IsDebugEnabled)}");
                }
                else
                {
                    _portalManager.AddGroupToUser(user, group);
                    _portalManager.SaveChanges();

                    Logger.Info($"+ {user.Format(Logger.IsDebugEnabled)}");
                    _portalManager.Detach(user);
                }
            }

            // need to detach the group 
            _portalManager.Detach(group);
        }

        public void DeleteUsersFromGroup(LicenseGroupUsersDto groupMembers)
        {
            LicenseGroup group = _portalManager.ListGroupById(groupMembers.Id);
            _portalManager.Container.AttachTo("LicenseGroups", group);

            List<LicenseUser> remoteUsers = _portalManager.ListAllUsersByGroupId(groupMembers.Id);

            IEnumerable<LicenseUser> staleMembers = remoteUsers.Except(ObjectMapper.Map<List<LicenseUser>>(groupMembers.Users), new LicenseUserComparer());

            foreach (LicenseUser staleMember in staleMembers)
            {
                _portalManager.DeleteGroupFromUser(staleMember, group);
                _portalManager.SaveChanges();

                Logger.Info($"- {staleMember.Format(Logger.IsDebugEnabled)}");
                _portalManager.Detach(staleMember);
            }

            // need to detach the group
            _portalManager.Detach(group);
        }
    }
}