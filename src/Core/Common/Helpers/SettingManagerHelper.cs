namespace Core.Common.Helpers
{
    using System;
    using Abp;
    using Administration;
    using Extensions;

    public class SettingManagerHelper
    {
        private static readonly SettingManager SettingManager = new SettingManager();
        private static int _accountId;

        private static Guid _deviceId;

        public static int AccountId
        {
            get
            {
                if (_accountId == default(int))
                {
                    int dbLookup = SettingManager.GetSettingValue<int>(SettingNames.AutotaskAccountId);
                    if (dbLookup == default(int))
                    {
                        throw new AbpException("Account id is invalid.");
                    }

                    _accountId = dbLookup;
                }

                return _accountId;
            }
        }

        public static Guid DeviceId
        {
            get
            {
                if (_deviceId == default(Guid))
                {
                    var dbLookup = SettingManager.GetSettingValue<Guid>(SettingNames.CentrastageDeviceId);
                    if (dbLookup == default(Guid))
                    {
                        throw new AbpException("Device id is invalid.");
                    }

                    _deviceId = dbLookup;
                }

                return _deviceId;
            }
        }
    }
}