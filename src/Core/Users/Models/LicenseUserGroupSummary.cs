using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Users.Models
{
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
