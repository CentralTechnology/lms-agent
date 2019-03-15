namespace LMS.Core.Veeam.Factory
{
    using System;
    using FluentResults;
    using Portal.LicenseMonitoringSystem.Veeam.Entities;

    public abstract class VeeamCreator
    {
        protected readonly Version ApplicationVersion;

        protected VeeamCreator(Version applicationVersion)
        {
            ApplicationVersion = applicationVersion;
        }

        public abstract Result<Veeam> Create();
    }
}