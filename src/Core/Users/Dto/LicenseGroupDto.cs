namespace LMS.Users.Dto
{
    using System;
    using Abp.Application.Services.Dto;

    public class LicenseGroupDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public DateTimeOffset WhenCreated { get; set; }
    }
}