namespace Core.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Abp.Timing;
    using Users.Entities;

    public class LicenseGroup : LicenseBase
    {
        private DateTime _whenCreated;

        [MaxLength(64)]
        public string Name { get; set; }

        public DateTime WhenCreated
        {
            get => _whenCreated;
            set => _whenCreated = Clock.Normalize(value);
        }
    }
}