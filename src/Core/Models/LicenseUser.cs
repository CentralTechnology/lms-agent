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

        public int ManagedSupportId { get; set; }

        [MaxLength(64)]
        public string Surname { get; set; }

        public DateTime WhenCreated { get; set; }
    }

    [Flags]
    public enum CallInStatus
    {
        CalledIn = 0,
        NotCalledIn = 1,
        NeverCalledIn = 2
    }
}