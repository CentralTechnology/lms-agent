namespace LMS.Users.Dto
{
    using System;
    using System.Collections.Generic;
    using Abp.Application.Services.Dto;

    public class GroupPrincipalOutput : EntityDto<Guid>
    {
        public List<Guid> Members { get; set; }
        public string Name { get; set; }
        public DateTime WhenCreated { get; set; }
    }
}