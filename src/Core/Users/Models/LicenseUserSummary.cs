using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Users.Models
{
    using Abp.Application.Services.Dto;

    public class LicenseUserSummary : EntityDto<Guid>
    {
        public string DisplayName { get; set; }

        public override string ToString()
        {
            return $"User: {DisplayName}  Identifier: {Id}";
        }

        public LicenseUserGroupStatus Status { get; set; }
    }

    public enum LicenseUserGroupStatus
    {
        Added,
        Removed
    }
}
