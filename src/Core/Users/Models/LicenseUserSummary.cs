namespace LMS.Users.Models
{
    using System;
    using Abp.Application.Services.Dto;

    public class LicenseUserSummary : EntityDto<Guid>
    {
        public string DisplayName { get; set; }

        public LicenseUserGroupStatus Status { get; set; }

        public override string ToString()
        {
            return $"User: {DisplayName}  Identifier: {Id}";
        }
    }

    public enum LicenseUserGroupStatus
    {
        Added,
        Removed
    }
}