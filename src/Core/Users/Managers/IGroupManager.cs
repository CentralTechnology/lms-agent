using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Users.Managers
{
    using Abp.Domain.Services;
    using Dto;

    public interface IGroupManager : IDomainService
    {
        void Add(LicenseGroupDto input, int tenantId);
        void Update(LicenseGroupDto input);
        void Delete(Guid id);
    }
}
