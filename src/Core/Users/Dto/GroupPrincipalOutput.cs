using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Users.Dto
{
    using Abp.Application.Services.Dto;

    public class GroupPrincipalOutput : EntityDto<Guid>
    {
        public string Name { get; set; }
        public List<Guid> Members { get; set; }
        public DateTime WhenCreated { get; set; }
    }
}
