namespace LMS.Common.Helpers
{
    using System;
    using System.Reflection;
    using Core.Administration;
    using Core.Configuration;
    using Extensions;
    using NLog;

    public class SettingManagerHelper
    {
        private readonly Logger _logger;
        private readonly SettingManager _settingManager;

        private int _accountId;

        private Guid _deviceId;

        public  SettingManagerHelper()
        {
            _logger = LogManager.GetCurrentClassLogger();
            _settingManager = new SettingManager();
        }

        [Obsolete]
        public virtual int AccountId
        {
            get
            {
                if (_accountId == default(int))
                {
                    int dbLookup = _settingManager.GetSettingValue<int>(AppSettingNames.AutotaskAccountId);

                    _accountId = dbLookup;
                }

                return _accountId;
            }
        }

        public virtual string ClientVersion
        {
            get
            {
                try
                {
                    return Assembly.GetEntryAssembly().GetName().Version.ToString();
                }
                catch (Exception ex)
                {
                    _logger.Error("Unable to determine client version.");
                    _logger.Debug(ex);
                }

                return string.Empty;
            }
        }

        [Obsolete]
        public virtual Guid DeviceId
        {
            get
            {
                if (_deviceId == default(Guid))
                {
                    var dbLookup = _settingManager.GetSettingValue<Guid>(AppSettingNames.CentrastageDeviceId);

                    _deviceId = dbLookup;
                }

                return _deviceId;
            }
        }

        public static SettingManagerHelper Instance => _instance ?? (_instance = new SettingManagerHelper());

        private static SettingManagerHelper _instance;

        public static void SetTestingInstance(SettingManagerHelper newInstance)
        {
            _instance = newInstance;
        }

    }
}