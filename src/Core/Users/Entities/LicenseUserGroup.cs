// ReSharper disable CheckNamespace
namespace Portal.LicenseMonitoringSystem.Users.Entities
{
    using System;

    public partial class LicenseUserGroup
    {
        public static LicenseUserGroup Create(Guid groupId, Guid userId)
        {
            return new LicenseUserGroup
            {
                GroupId = groupId,
                UserId = userId
            };
        }
    }
}