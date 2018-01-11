namespace LMS.Users.Dto
{
    using System;
    using Abp.Application.Services.Dto;

    public class LicenseUserDto : EntityDto<Guid>
    {
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public bool Enabled { get; set; }
        public string FirstName { get; set; }
        public DateTimeOffset? LastLogonDate { get; set; }
        public string SamAccountName { get; set; }
        public string Surname { get; set; }
        public DateTimeOffset WhenCreated { get; set; }
    }
}