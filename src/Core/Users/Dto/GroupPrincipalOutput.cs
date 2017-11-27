namespace LMS.Users.Dto
{
    using System;
    using System.Collections.Generic;
    using Abp.Application.Services.Dto;

    public class GroupPrincipalOutput : EntityDto<Guid>
    {
        public string Name { get; set; }
        public List<Guid> Members { get; set; }
        public DateTime WhenCreated { get; set; }
    }
}
