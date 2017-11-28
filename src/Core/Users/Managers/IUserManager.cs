using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Users.Managers
{
    using Abp.Domain.Services;
    using Dto;

    public interface IUserManager : IDomainService
    {
        /// <summary>
        /// Add a License User to the API.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="managedSupportId"></param>
        /// <param name="tenantId"></param>
        void Add(LicenseUserDto input, int managedSupportId, int tenantId);

        /// <summary>
        /// Update a License User from the API.
        /// </summary>
        /// <param name="input"></param>
        void Update(LicenseUserDto input);

        /// <summary>
        /// Delete a License User from the API.
        /// </summary>
        /// <param name="id"></param>
        void Delete(Guid id);
    }
}
