namespace LMS.Core.Services.Authentication
{
    using System;
    using Abp.Domain.Services;

    public interface IPortalAuthenticationService : IDomainService
    {
        long GetAccount();
        long GetAccount(Guid device);
        Guid GetDevice();
        string GetToken();
    }
}