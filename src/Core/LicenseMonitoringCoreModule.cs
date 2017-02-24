namespace Core
{
    using System.Reflection;
    using Abp.AutoMapper;
    using Abp.Modules;
    using AutoMapper;

    [DependsOn(typeof(AbpAutoMapperModule))]
    public class LicenseMonitoringCoreModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        public override void PostInitialize()
        {
            base.PostInitialize();

            //Mapper.Initialize(configuration =>
            //{
            //    configuration.CreateMap<LicenseUser, User>();
            //    configuration.CreateMap<User, LicenseUser>(MemberList.Source)
            //        .ForMember(src => src.Groups, opt => opt.Ignore());
            //    configuration.CreateMap<LicenseUserGroup, UserGroup>();
            //    configuration.CreateMap<UserGroup, LicenseUserGroup>(MemberList.Source);
            //});

            Mapper.AssertConfigurationIsValid();
        }
    }
}