namespace Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Abp.Domain.Entities;

    public class ManagedSupport : Entity
    {
        public DateTime CheckInTime { get; set; }

        [Required]
        public Guid DeviceId { get; set; }

        [MaxLength(200)]
        public string Hostname { get; set; }

        [MaxLength(39)]
        public string IpAddress { get; set; }

        public bool IsActive { get; set; }

        public CallInStatus Status { get; set; }
        public int TenantId { get; set; }

        [Required]
        public int UploadId { get; set; }

        public List<LicenseUser> Users { get; set; }
    }
}