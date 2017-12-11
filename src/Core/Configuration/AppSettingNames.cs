namespace LMS.Configuration
{
    public class AppSettingNames
    {
        /// <summary>
        ///     Autotask Account ID
        /// </summary>
        public const string AutotaskAccountId = "Setting.Autotask.Id";

        /// <summary>
        ///     CentraStage Device ID
        /// </summary>
        public const string CentrastageDeviceId = "Setting.Centrastage.Id";

        /// <summary>
        /// 
        /// </summary>
        public const string ManagedSupportId = "Setting.Users.ManagedSupportId";

        /// <summary>
        ///     Automatic start up process set's this value
        /// </summary>
        public const string MonitorUsers = "Monitor.Users";

        /// <summary>
        ///     Automatic start up process set's this value
        /// </summary>
        public const string MonitorVeeam = "Monitor.Veeam";

        /// <summary>
        ///     Allows the lms agent to run on a non Primary Domain Controller
        /// </summary>
        public const string PrimaryDomainControllerOverride = "Setting.Users.PrimaryDomainControllerOverride";

        /// <summary>
        ///     Caches the version of Veeam installed
        /// </summary>
        public const string VeeamVersion = "Setting.Veeam.Version";

        /// <summary>
        ///     Override to disable Veeam monitoring
        /// </summary>
        public const string VeeamOverride = "Setting.Veeam.Override";

        /// <summary>
        /// Override to disable User monitoring
        /// </summary>
        public const string UsersOverride = "Setting.Users.Override";
    }
}