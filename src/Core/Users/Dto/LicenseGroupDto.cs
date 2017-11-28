using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Users.Dto
{
    using Abp.Application.Services.Dto;

    public class LicenseGroupDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public DateTimeOffset WhenCreated { get; set; }
    }
}
