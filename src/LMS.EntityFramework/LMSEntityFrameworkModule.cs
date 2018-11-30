namespace LMS
{
    using System;
    using System.Data.Entity.Migrations;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Abp.EntityFramework;
    using Abp.Logging;
    using Abp.Modules;
    using Core;
    using Core.Helpers;

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
            
            var dir = Directory.GetCurrentDirectory();
            string connectionString = GetDatabaseLocation(dir);

            Configuration.DefaultNameOrConnectionString = $"DataSource={connectionString};LCID=2057";
            Configuration.UnitOfWork.IsTransactional = false;
        }

        private string GetDatabaseLocation(string directory)
        {
            if (directory.EndsWith("License Monitoring System") || directory.EndsWith("src"))
            {
                return Path.Combine(directory, "Data", DatabaseName);
            }

            var files = Directory.GetFiles(directory, DatabaseName);
            if (!files.Any())
            {
                var parent = Directory.GetParent(directory);
                return GetDatabaseLocation(parent.FullName);
            }

            return Path.Combine(directory, DatabaseName);
        }
    }
}