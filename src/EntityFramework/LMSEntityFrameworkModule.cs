namespace LMS
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Reflection;
    using Abp.EntityFramework;
    using Abp.Logging;
    using Abp.Modules;

    [DependsOn(typeof(AbpEntityFrameworkModule), typeof(LMSCoreModule))]
    public class LMSEntityFrameworkModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            base.Initialize();
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = "Default";
            Configuration.UnitOfWork.IsTransactional = false;
        }
    }
}