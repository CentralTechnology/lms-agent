using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Users.Models
{
    using Abp.Application.Services.Dto;

    public class LicenseGroupSummary : EntityDto<Guid>
    {
        public string Name { get; set; }

        public override string ToString()
        {
            return $"Group: {Name}  Identifier: {Id}";
        }
    }
}
