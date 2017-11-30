namespace LMS.Veeam.Models
{
    using System;
    using Portal.Common.Enums;
    using Portal.LicenseMonitoringSystem.Veeam.Entities;

    public class VeeamUpdateModel
    {
        public DateTimeOffset CheckInTime { get; set; }
        public string ClientVersion { get; set; }
        public LicenseEditions Edition { get; set; }
        public DateTimeOffset ExpirationDate { get; set; }
        public int HyperV { get; set; }
        public string ProgramVersion { get; set; }
        public CallInStatus Status { get; set; }
        public string SupportId { get; set; }
        public int UploadId { get; set; }

        public int vSphere { get; set; }
    }
}