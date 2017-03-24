using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    using Abp.Domain.Services;
    using Common.Enum;

    public interface IOrchestratorManager : IDomainService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="monitor"></param>
        void Run(Monitor monitor);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task UserMonitor();
    }
}
