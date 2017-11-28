using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Users.Profiles
{
    using AutoMapper;
    using Dto;
    using Portal.LicenseMonitoringSystem.Users.Entities;

    public class LicenseGroupProfile : Profile
    {
        public LicenseGroupProfile()
        {
            CreateMap<LicenseGroupDto, LicenseGroup>()
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Users, opt => opt.Ignore());
        }
    }
}
