using System;

namespace LMS.Core.Veeam.Backup.Common
{
    [Serializable]
    public enum ELicensePlatform
    {
        VmWare = 0,
        HyperV = 1,
        EpWindows = 6,
        EpLinux = 7,
        NutanixAHV = 100, // 0x00000064
        N2W = 200, // 0x000000C8
        PluginServers = 300, // 0x0000012C
        CloudBackup = 10000, // 0x00002710
        CloudReplica = 10001, // 0x00002711
    }
}
