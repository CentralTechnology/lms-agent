﻿namespace LMS.Users.Profiles
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