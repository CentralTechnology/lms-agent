namespace LMS.Core.Veeam.Managers
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using Abp.Configuration;
    using Abp.Domain.Services;
    using Configuration;
    using Helpers;
    using Serilog;

    public class VeeamManager : DomainService, IVeeamManager
    {
        public const string Veeam90FilePath = @"C:\Program Files\Veeam\Backup and Replication\Veeam.Backup.Service.exe";

        public const string VeeamFilePath = @"C:\Program Files\Veeam\Backup and Replication\Backup\Veeam.Backup.Service.exe";

        private readonly ILogger _logger = Log.ForContext<VeeamManager>();

        public bool IsInstalled()
        {
            const string veeamApplicationName = "Veeam Backup & Replication Server";

            try
            {
                return CommonHelpers.IsApplictionInstalled(veeamApplicationName);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Debug(ex, ex.Message);
                return false;
            }
        }

        public bool IsOnline()
        {
            IPAddress localhost = IPAddress.Parse("127.0.0.1");

            using (var tcpClient = new TcpClient())
            {
                try
                {
                    tcpClient.Connect(localhost, 9392);
                    return true;
                }
                catch (SocketException)
                {
                    return false;
                }
            }
        }

        public Version GetInstalledVeeamVersion()
        {
            if (File.Exists(VeeamFilePath))
            {
                FileVersionInfo version = FileVersionInfo.GetVersionInfo(VeeamFilePath);
                SettingManager.ChangeSettingForApplication(AppSettingNames.VeeamVersion, version.FileVersion);
                return Version.Parse(version.FileVersion);
            }

            if (File.Exists(Veeam90FilePath))
            {
                FileVersionInfo version = FileVersionInfo.GetVersionInfo(Veeam90FilePath);
                SettingManager.ChangeSettingForApplication(AppSettingNames.VeeamVersion, version.FileVersion);
                return Version.Parse(version.FileVersion);
            }

            SettingManager.ChangeSettingForApplication(AppSettingNames.VeeamVersion, string.Empty);
            return null;
        }
    }
}