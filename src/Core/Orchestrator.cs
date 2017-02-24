namespace Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Abp.AutoMapper;
    using Abp.Dependency;
    using Abp.Timing;
    using Common.Client;
    using Common.Extensions;
    using Models;
    using Settings;
    using Users;

    public class Orchestrator : LicenseMonitoringBase, ISingletonDependency
    {
        private readonly IUserClient _userClient;
        private readonly IUserGroupClient _userGroupClient;
        private readonly IUserManager _userManager;
        private readonly ISupportUploadClient _supportUploadClient;

        public Orchestrator(
            IUserManager userManager,
            IUserClient userClient,
            IUserGroupClient userGroupClient,
            ISupportUploadClient supportUploadClient)
        {
            _userManager = userManager;
            _userClient = userClient;
            _userGroupClient = userGroupClient;
            _supportUploadClient = supportUploadClient;
        }


    }
}