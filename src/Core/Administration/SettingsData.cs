namespace Core.Administration
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using Abp.Extensions;
    using Common.Enum;
    using System.Linq;

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
        /// </summary>
        public const string MonitorsName = "Monitors";

        [ConfigurationProperty(AccountIdName)]
        public int AccountId
        {
            get => (int) this[AccountIdName];
            set => this[AccountIdName] = value;
        }

        [ConfigurationProperty(DeviceIdName)]
        public Guid DeviceId
        {
            get => (Guid) this[DeviceIdName];
            set => this[DeviceIdName] = value;
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
                else
                {
                    // ReSharper disable once PossibleInvalidCastException
                    return (Monitor) this[MonitorsName];
                }

            }
            set => this[MonitorsName] = value;
        }
    }
}