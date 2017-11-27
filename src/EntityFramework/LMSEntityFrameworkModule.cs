namespace LMS
{
    using System.Data.Entity;
    using System.Reflection;
    using Abp.EntityFramework;
    using Abp.Modules;
    using EntityFramework;
    using Core;

    [DependsOn(typeof(AbpEntityFrameworkModule),typeof(LMSCoreModule))]
    public class LMSEntityFrameworkModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        public override void PreInitialize()
        {
        //    Database.SetInitializer(new CreateDatabaseIfNotExists<LMSDbContext>());

            Configuration.DefaultNameOrConnectionString = "Default";
        }
    }
}
