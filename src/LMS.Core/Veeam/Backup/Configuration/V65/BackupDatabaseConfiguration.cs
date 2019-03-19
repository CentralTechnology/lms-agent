using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
    internal class BackupDatabaseConfiguration
    {
        private static IDatabaseConfigurationInfo Default
        {
            get
            {
                return (IDatabaseConfigurationInfo) new DatabaseConfigurationInfo("(local)", "VEEAM", string.Empty, -1, "VeeamBackup", string.Empty, 11, 20, 120, 15, 180, (int) TimeSpan.FromHours(6.0).TotalSeconds, string.Empty, string.Empty, string.Empty, true, false, string.Empty, 1000);
            }
        }

        public static IDatabaseConfiguration Initialize(
            string registryKey,
            RegistryView registryView = RegistryView.Default)
        {
            return DatabaseConfiguration.Create(registryKey, BackupDatabaseConfiguration.Default, registryView);
        }

        public static IDatabaseConfiguration Load(
            string registryKey,
            RegistryView registryView = RegistryView.Default)
        {
            return DatabaseConfiguration.Load(registryKey, BackupDatabaseConfiguration.Default, registryView);
        }
    }
}
