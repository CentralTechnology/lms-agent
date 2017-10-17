using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    using Abp.Application.Services.Dto;

    public class LicenseGroupUpdateModel : EntityDto<Guid>
    {
        public string Name { get; set; }
        public DateTimeOffset WhenCreated { get; set; }

    }
}
