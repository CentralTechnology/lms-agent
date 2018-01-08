namespace LMS.Veeam.Models
{
    using System;
    using Backup.Common;
    using Enums;

    public class VmLicensingInfo
    {
        public static readonly TimeSpan ManagedVmTimeout = TimeSpan.FromDays(31.0);

        public VmLicensingInfo(Guid objectId, DateTime? firstOibCreationTime, DateTime? lastOibCreationTime, EPlatform platform, string hostName, string backupServerName, string vmName)
        {
            VmName = vmName;
            State = ComputeState(firstOibCreationTime, lastOibCreationTime);
            FirstOibCreationTime = firstOibCreationTime;
            Platform = platform;
            ObjectId = objectId;
            HostName = hostName;
            BackupServerName = backupServerName;
        }

        public string BackupServerName { get; private set; }

        public DateTime? FirstOibCreationTime { get; private set; }

        public string HostName { get; private set; }

        public Guid ObjectId { get; private set; }

        public EPlatform Platform { get; private set; }

        public EVmLicensingStatus State { get; private set; }

        public string VmName { get; private set; }

        private EVmLicensingStatus ComputeState(DateTime? firstStartTime, DateTime? lastStartTime)
        {
            if (!firstStartTime.HasValue)
                return EVmLicensingStatus.NotRegistered;
            DateTime utcNow = DateTime.UtcNow;
            DateTime? nullable1 = lastStartTime;
            TimeSpan managedVmTimeout = ManagedVmTimeout;
            DateTime? nullable2 = nullable1.HasValue ? nullable1.GetValueOrDefault() + managedVmTimeout : new DateTime?();
            return (nullable2.HasValue ? (utcNow > nullable2.GetValueOrDefault() ? 1 : 0) : 0) != 0 ? EVmLicensingStatus.Expired : EVmLicensingStatus.Managed;
        }

        public static VmLicensingInfo CreateForEnterpise(Guid objectId, DateTime firstStartTime, DateTime lastStartTime, EPlatform platform, string hostName, string backupServerName, string vmName)
        {
            return new VmLicensingInfo(objectId, firstStartTime, lastStartTime, platform, hostName, backupServerName, vmName);
        }

        public override string ToString()
        {
            return $"ObjectId: {(object) ObjectId}, HostName: {(object) HostName}, BackupServerName: {(object) BackupServerName}, Platform: {(object) Platform}";
        }
    }
}