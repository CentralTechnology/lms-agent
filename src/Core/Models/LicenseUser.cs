namespace Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Abp.Domain.Entities;

    public class LicenseUser : Entity<Guid>
    {
        [MaxLength(256)]
        public string DisplayName { get; set; }

        [MaxLength(256)]
        public string Email { get; set; }

        public bool Enabled { get; set; }

        [MaxLength(64)]
        public string FirstName { get; set; }

        public List<LicenseGroup> Groups { get; set; }

        public int SupportUploadId { get; set; }

        [MaxLength(64)]
        public string Surname { get; set; }

        public DateTime WhenCreated { get; set; }
    }

    public class LicenseGroup : Entity<Guid>
    {
        [MaxLength(64)]
        public virtual string Name { get; set; }

        public virtual DateTime WhenCreated { get; set; }
    }

    public class SupportUpload : Entity
    {
        public virtual DateTime CheckInTime { get; set; }

        [Required]
        public virtual Guid DeviceId { get; set; }

        [MaxLength(200)]
        public virtual string Hostname { get; set; }

        [MaxLength(39)]
        public virtual string IpAddress { get; set; }

        public bool IsActive { get; set; }

        public virtual CallInStatus Status { get; set; }
        public int TenantId { get; set; }

        public virtual double? Threshold { get; set; }

        [Required]
        public virtual int UploadId { get; set; }

        public virtual List<LicenseUser> Users { get; set; }
    }

    [Flags]
    public enum CallInStatus
    {
        CalledIn = 0,
        NotCalledIn = 1,
        NeverCalledIn = 2
    }
}