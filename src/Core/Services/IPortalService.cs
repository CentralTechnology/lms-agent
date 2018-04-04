using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Services
{
    using Abp.Domain.Services;
    using Microsoft.OData.Client;
    using Portal.LicenseMonitoringSystem.Veeam.Entities;

    public interface IPortalService : IDomainService
    {
        DataServiceCollection<Veeam> GetVeeamServer();
        Task UpdateVeeamServerAsync(Veeam update);
    }
}
