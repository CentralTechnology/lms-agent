namespace LMS.Users.Models
{
    using System;
    using Abp.Application.Services.Dto;

    public class LicenseGroupSummary : EntityDto<Guid>
    {
        public string Name { get; set; }

        public override string ToString()
        {
            return $"Group: {Name}  Identifier: {Id}";
        }
    }
}