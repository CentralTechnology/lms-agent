using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Users.Dto
{
    using Abp.Application.Services.Dto;

    public class LicenseGroupUsersDto : EntityDto<Guid>
    {
        public LicenseGroupUsersDto(Guid groupId, string groupName)
        {
            Id = groupId;
            GroupName = groupName;
            Users = new List<LicenseUserDto>();
        }
        public string GroupName { get; set; }
        public List<LicenseUserDto> Users { get; set; }
    }
}
