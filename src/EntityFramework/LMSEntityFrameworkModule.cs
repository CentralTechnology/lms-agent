namespace LMS
{
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
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

            base.Initialize();
        }

        public override void PreInitialize()
        {
            var dbMigrator = new DbMigrator(new Migrations.Configuration());
            dbMigrator.Update();

            Configuration.DefaultNameOrConnectionString = "Default";
            Configuration.UnitOfWork.IsTransactional = false;
        }
    }
}
