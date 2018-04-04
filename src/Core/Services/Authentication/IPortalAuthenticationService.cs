using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Services.Authentication
{
    using Abp.Domain.Services;

    public interface IPortalAuthenticationService : IDomainService
    {
        string GetToken();

        long GetAccount();
        long GetAccount(Guid device);
        Guid GetDevice();
    }
}
