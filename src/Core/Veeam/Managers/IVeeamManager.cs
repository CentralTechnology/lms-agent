using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Veeam.Managers
{
    using Abp.Domain.Services;
    using Portal.LicenseMonitoringSystem.Veeam.Entities;

    public interface IVeeamManager: IDomainService
    {
        Veeam GetLicensingInformation(Veeam veeam);

        bool IsInstalled();

        string GetVersion();
        bool IsOnline();
    }
}
