using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseMonitoringSystem.Core.Users
{
    using System.ComponentModel.DataAnnotations;
    using Abp.Application.Services.Dto;
    using Abp.Domain.Entities;

    public class User: Entity<Guid>
    {
        [StringLength(256)]
        public string DisplayName { get; set; }

        [StringLength(256)]
        public string Email { get; set; }

        public bool Enabled { get; set; }

        [StringLength(64)]
        public string FirstName { get; set; }
        public List<UserGroup> Groups { get; set; }
        [StringLength(64)]
        public string Surname { get; set; }

        public DateTimeOffset WhenCreated { get; set; }
    }

    public class UserGroup : Entity<Guid>
    {
        [StringLength(64)]
        public string Name { get; set; }

        public DateTimeOffset WhenCreated { get; set; }

        public Guid UserId { get; set; }
    }
}
