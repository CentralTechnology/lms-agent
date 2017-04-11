namespace Core.Administration
{
    using System;
    using System.Configuration;
    using Common.Enum;

    public class SettingsData : ConfigurationSection
    {
        /// <summary>
        ///     Used to identify the <see cref="AccountId" /> in the configuration
        /// </summary>
        public const string AccountIdName = "AccountId";

        /// <summary>
        ///     Used to identify the <see cref="DeviceId" /> in the configuration
        /// </summary>
        public const string DeviceIdName = "DeviceId";

        /// <summary>
        /// Used to identify the <see cref="Monitors" /> in the configuration
        /// </summary>
        public const string MonitorsName = "Monitors";

        [ConfigurationProperty(AccountIdName)]
        public int AccountId
        {
            get
            {
                return (int)this[AccountIdName];
            }
            set
            {
                this[AccountIdName] = value;
            }
        }

        [ConfigurationProperty(DeviceIdName)]
        public Guid DeviceId
        {
            get
            {
                return (Guid)this[DeviceIdName];
            }
            set
            {
                this[DeviceIdName] = value;
            }
        }

        [ConfigurationProperty(MonitorsName)]
        public Monitor Monitors
        {
            get
            {
                var mon = this[MonitorsName];
                if (string.IsNullOrEmpty(mon as string))
                {
                    return Monitor.Users;
                }

                return (Monitor)this[MonitorsName];
            }
            set
            {
                this[MonitorsName] = value;
            }
        }


    }
}