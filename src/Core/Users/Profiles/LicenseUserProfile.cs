namespace LMS.Users.Profiles
{
    using AutoMapper;
    using Dto;
    using Portal.LicenseMonitoringSystem.Users.Entities;

    public class LicenseUserProfile : Profile
    {
        public LicenseUserProfile()
        {
            CreateMap<LicenseUserDto, LicenseUser>()
                .ForMember(dest => dest.UserGroups, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.ManagedSupportId, opt => opt.Ignore())
                .ForMember(dest => dest.TenantId, opt => opt.Ignore())
                .ForMember(dest => dest.UserType, opt => opt.Ignore());
        }
    }
}