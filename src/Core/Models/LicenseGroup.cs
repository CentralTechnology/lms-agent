namespace Core.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Abp.Domain.Entities;
    using Users.Entities;

    public class LicenseGroup : LicenseBase
    {
        [MaxLength(64)]
        public string Name { get; set; }

        public DateTime WhenCreated { get; set; }
    }
}