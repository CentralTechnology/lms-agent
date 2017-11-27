namespace LMS.EntityFramework
{
    using System.Data.Common;
    using System.Data.Entity;
    using Abp.EntityFramework;
    using Configuration;
    using Migrations;

    public class LMSDbContext : AbpDbContext
    {
        /// <summary>
        /// Settings.
        /// </summary>
        public virtual IDbSet<Setting> Settings { get; set; }

        public LMSDbContext()
            : base("Default")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<LMSDbContext, Configuration>());
        }

        public LMSDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        public LMSDbContext(DbConnection existingConnection)
            : base(existingConnection, false)
        {

        }

        public LMSDbContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {

        }
    }
}
