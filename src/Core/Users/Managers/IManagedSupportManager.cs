using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Users.Managers
{
    using Abp.Domain.Services;
    using Portal.LicenseMonitoringSystem.Users.Entities;

    public interface IManagedSupportManager : IDomainService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ManagedSupport Get();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ManagedSupport Add();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        void Update(ManagedSupport input);
    }
}
