namespace Core.Common.Constants
{
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Constants
    {
        /// <summary>
        ///     Veeam registry Display Name
        /// </summary>
        public const string VeeamApplicationName = "Veeam Backup & Replication Server";

        /// <summary>
        ///     Centrastage Device Id registry key name
        /// </summary>
        public const string CentraStage = "CentraStage";

        /// <summary>
        ///     Veeam Backup & Replication: 9.0.0.902
        /// </summary>
        public const string VeeamVersion900902 = "9.0.0.902";

        /// <summary>
        ///     Veeam Backup & Replication: 9.5.0.1038
        /// </summary>
        public const string VeeamVersion9501038 = "9.5.0.1038";

        public const int VeeamProtectedVmCountDays = 31;

#if DEBUG /// <summary>
///     Base url for the api client
/// </summary>
        public const string BaseServiceUrl = "http://localhost:61814/";

#else
        /// <summary>
        ///     Base url for the api client
        /// </summary>
        public const string BaseServiceUrl = "https://portal.ct.co.uk";
#endif

        /// <summary>
        ///     Doesn't actually provide any admin access. Its just used to hide menu items.
        /// </summary>
        public const string AdminAccess = "a490d701-b267-4d4a-87bc-fe0d0f4c9105";

#if DEBUG /// <summary>
///     Default endpoint for OData
/// </summary>
        public const string DefaultServiceUrl = "http://localhost:61814/odata/v1";
#else
        /// <summary>
        ///     Default endpoint for OData
        /// </summary>
        public const string DefaultServiceUrl = "https://portal.ct.co.uk/odata/v1";

#endif

        public const string ServiceDisplayName = "License Monitoring System";
        public const string ServiceName = "LicenseMonitoringSystem";
        public const string ServiceDescription = "A tool used to monitor various licenses.";

        public const string SentryDSN = "https://1fbf4ff918e144cdb86e5f71cacb7650:8f3633dc11734def9a1cd3f833259a91@sentry.io/198304";
    }
}