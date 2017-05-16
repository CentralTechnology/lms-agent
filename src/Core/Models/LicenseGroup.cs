namespace Core.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Abp.Domain.Entities;

    public class LicenseGroup : Entity<Guid>
    {
        [MaxLength(64)]
        public virtual string Name { get; set; }

        public virtual DateTime WhenCreated { get; set; }
    }
}