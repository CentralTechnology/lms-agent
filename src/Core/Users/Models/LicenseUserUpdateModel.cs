using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    using Abp.Application.Services.Dto;

    public class LicenseUserUpdateModel : EntityDto<Guid>
    {
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public bool Enabled { get; set; }
        public string FirstName { get; set; }
        public DateTimeOffset? LastLoginDate { get; set; }
        public string SamAccountName { get; set; }
        public string Surname { get; set; }
        public DateTimeOffset WhenCreated { get; set; }

    }
}
