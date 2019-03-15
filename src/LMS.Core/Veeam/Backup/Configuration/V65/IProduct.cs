using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Core.Veeam.Backup.Configuration.V80;
using Microsoft.Win32;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
    public interface IProduct
    {
        Guid Id { get; }

        string Name { get; }

        string Alias { get; }

        string DisplayName { get; }

        string RegistryKey { get; }

        Version AssemblyVersion { get; }

        Version ProductVersion { get; }

        bool IsPreview { get; }

        Version UpgradableVersion { get; }

        Version MinSupportedServerVersion { get; }

        IEnumerable<IProductRelease> LoadReleases(
            Version productVersion,
            IDatabaseVersion productDatabaseVersion);

        IEnumerable<IProductSetup> LoadSetups(
            Version productVersion,
            Guid productPackageCode);

        IProductProcesses LoadProcesses();

        IEnumerable<IProductServer> LoadServers();

        IRegistryConfigurationController CreateRegistryController(
            bool writable,
            RegistryView registryView = RegistryView.Default);

        IEnumerable<string> DatabaseDeployers { get; }

        string DatabaseFileName { get; }

        string ProductFolderName { get; }

        string DatabaseLockId { get; }

        EventLogSource EventLogSource { get; }

        IDatabaseConfiguration CreateDatabaseConfiguration(
            RegistryView registryView = RegistryView.Default);

        IDatabaseConfiguration LoadDatabaseConfiguration(RegistryView registryView = RegistryView.Default);

        string MainHelpUrl { get; }

        string AdvancedHelpUrl { get; }
    }
}
