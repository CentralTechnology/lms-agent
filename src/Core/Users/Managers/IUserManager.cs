namespace LMS.Users.Managers
{
    using System;
    using Abp.Domain.Services;
    using Dto;
    using global::Hangfire.Server;

    public interface IUserManager : IDomainService
    {
        /// <summary>
        ///     Add a License User to the API.
        /// </summary>
        /// <param name="performContext"></param>
        /// <param name="input"></param>
        /// <param name="managedSupportId"></param>
        /// <param name="tenantId"></param>
        void Add(PerformContext performContext, LicenseUserDto input, int managedSupportId, int tenantId);

        /// <summary>
        ///     Delete a License User from the API.
        /// </summary>
        /// <param name="performContext"></param>
        /// <param name="id"></param>
        void Delete(PerformContext performContext, Guid id);

        /// <summary>
        ///     Update a License User from the API.
        /// </summary>
        /// <param name="performContext"></param>
        /// <param name="input"></param>
        void Update(PerformContext performContext, LicenseUserDto input);
    }
}