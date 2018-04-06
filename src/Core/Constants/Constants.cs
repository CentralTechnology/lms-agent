namespace LMS.Core.Extensions.Constants
{
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Constants
    {
        public const string SentryDSN = "https://1fbf4ff918e144cdb86e5f71cacb7650:8f3633dc11734def9a1cd3f833259a91@sentry.io/198304";
        public const string ServiceDescription = "A tool used to monitor various licenses.";

        public const string ServiceDisplayName = "License Monitoring System";
        public const string ServiceName = "LicenseMonitoringSystem";
    }
}