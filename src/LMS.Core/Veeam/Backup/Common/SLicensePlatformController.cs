using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
    using LMS.Core.Common;

    public static class SLicensePlatformController
    {
        private static readonly HashSet<CLicensePlatform> _registeredPlatforms = new HashSet<CLicensePlatform>();

        public static CLicensePlatform Register(
            ELicensePlatform platform,
            EEpLicenseMode licenseMode)
        {
            return SLicensePlatformController.RegisterOrThrow(new CLicensePlatform(platform, licenseMode));
        }

        public static CLicensePlatform Register(ELicensePlatform platform)
        {
            return SLicensePlatformController.RegisterOrThrow(new CLicensePlatform(platform));
        }

        public static CLicensePlatform Get(
            ELicensePlatform platform,
            EEpLicenseMode licenseMode)
        {
            return SLicensePlatformController.GetOrThrow(new CLicensePlatform(platform, licenseMode));
        }

        public static CLicensePlatform Get(
            ELicensePlatform platform,
            EEpLicenseMode? licenseMode)
        {
            if (licenseMode.HasValue)
                return SLicensePlatformController.Get(platform, licenseMode.Value);
            return SLicensePlatformController.Get(platform);
        }

        public static CLicensePlatform Get(ELicensePlatform platform)
        {
            return SLicensePlatformController.GetOrThrow(new CLicensePlatform(platform));
        }

        private static CLicensePlatform RegisterOrThrow(CLicensePlatform platform)
        {
            if (!SLicensePlatformController._registeredPlatforms.Add(platform))
                throw ExceptionFactory.Create("Platform {0} already registered", (object) platform);
            return platform;
        }

        private static CLicensePlatform GetOrThrow(CLicensePlatform platform)
        {
            if (!SLicensePlatformController._registeredPlatforms.Contains(platform))
                throw ExceptionFactory.Create("Platform {0} not registered", (object) platform);
            return platform;
        }
    }
}
