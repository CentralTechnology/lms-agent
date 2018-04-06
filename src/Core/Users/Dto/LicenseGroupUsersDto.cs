﻿namespace LMS.Users.Dto
{
    using System;
    using System.Collections.Generic;
    using Abp.Application.Services.Dto;
    using Core.Users.Compare;
    using Portal.LicenseMonitoringSystem.Users.Entities;

    public class LicenseGroupUsersDto : EntityDto<Guid>
    {
        public LicenseGroupUsersDto(Guid groupId, string groupName)
        {
            Id = groupId;
            GroupName = groupName;
            Users = new HashSet<LicenseUser>(new LicenseUserEqualityComparer());
        }

        public string GroupName { get; set; }
        public HashSet<LicenseUser> Users { get; set; }
    }
}