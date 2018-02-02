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
            try
            {
                var dbMigrator = new DbMigrator(new Migrations.Configuration());
                dbMigrator.Update();

                Configuration.DefaultNameOrConnectionString = "Default";
                Configuration.UnitOfWork.IsTransactional = false;
            }
            catch (Exception ex)
            {
                LogHelper.Logger.Error(ex.Message, ex);
            }
        }
    }
}