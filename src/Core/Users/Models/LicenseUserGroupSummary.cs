namespace LMS.Users.Models
{
    using System;
    using System.Collections.Generic;
    using Abp.Application.Services.Dto;

    public class LicenseUserGroupSummary : EntityDto<Guid>
    {
        public LicenseUserGroupSummary()
        {
            Users = new List<LicenseUserSummary>();
        }
        public string Name { get; set; }

        public List<LicenseUserSummary> Users { get; set; }
    }  
}
