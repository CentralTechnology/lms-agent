using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
    public static class SOptions
    {
        private static volatile Lazy<COptions> _instance;
        private static DateTime _lastOptionsResetTime;

        static SOptions()
        {
            SOptions.Reset();
        }

        public static CCloudConnectOptionsScope CloudConnect
        {
            get
            {
                return SOptions.Instance.CloudConnect;
            }
        }

        public static void Reset()
        {
            SOptions._instance = new Lazy<COptions>(new Func<COptions>(SOptions.CreateInstance), true);
            SOptions._lastOptionsResetTime = SManagedDateTime.UtcNow;
        }

        public static COptions Instance
        {
            get
            {
                try
                {
                    if (DateTime.UtcNow.Duration(SOptions._lastOptionsResetTime) > TimeSpan.FromMinutes(5.0))
                        SOptions.Reset();
                    return SOptions._instance.Value;
                }
                catch (IOException)
                {
                    SOptions.Reset();
                    return SOptions._instance.Value;
                }
            }
        }

        private static COptions CreateInstance()
        {
            return new COptions();
        }

        public static void InitInstance()
        {
            COptions coptions = SOptions._instance.Value;
        }
    }
}
