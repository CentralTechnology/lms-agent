namespace LMS
{
    using System.IO;
    using System.Reflection;
    using Abp.EntityFramework;
    using Abp.Modules;
    using Core;

    [DependsOn(typeof(AbpEntityFrameworkModule), typeof(LMSCoreModule))]
    public class LMSEntityFrameworkModule : AbpModule
    {
        private const string DatabaseName = "Configuration.sdf";

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            base.Initialize();
        }

        public override void PreInitialize()
        {
            string dataPath = Path.Combine(Directory.GetCurrentDirectory(), "Data");
            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }

            string connectionString = Path.Combine(dataPath, DatabaseName);

            Configuration.DefaultNameOrConnectionString = $"DataSource={connectionString};LCID=2057";
            Configuration.UnitOfWork.IsTransactional = false;
        }
    }
}