namespace Core.Settings
{
    using System;

    public class Setting
    {
#if DEBUG
        public const string BaseServiceUrl = "http://localhost:61814/";
#else
        public const string BaseServiceUrl = "https://portal.ct.co.uk";
#endif
        public const string DefaultServiceUrl = "https://portal.ct.co.uk/odata/v1";
        public const string ServiceUrlProperty = "ServiceUrl";
        public const string DeviceIdKeyPath = @"SOFTWARE\CentraStage";
        public const string DeviceIdKeyName = "DeviceID";

        public Setting()
        {
            AccountId = 0;
            Monitor = new[]
            {
                Settings.Monitor.Users.ToString()
            };
#if DEBUG
            ServiceUrl = "http://localhost:61814/odata/v1";
#else
            ServiceUrl = DefaultServiceUrl;
#endif

            ResetOnStartUp = true;
        }

        public int? AccountId { get; set; }
        public bool? Debug { get; set; }

        public Guid? DeviceId { get; set; }

        public object this[string propertyName]
        {
            get { return GetType().GetProperty(propertyName).GetValue(this, null); }
            set { GetType().GetProperty(propertyName).SetValue(this, value, null); }
        }

        public string[] Monitor { get; set; }
        public bool ResetOnStartUp { get; set; }
        public string ServiceUrl { get; set; }
    }

    public enum Monitor
    {
        Users
    }
}