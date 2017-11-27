namespace Core.EntityFramework
{
    using System.Data.Common;
    using System.Data.Entity;
    using Administration;
    using Migrations;

    public class AgentDbContext : DbContext
    {
        public AgentDbContext()
            : base("Default")
        {
           // Database.SetInitializer(new MigrateDatabaseToLatestVersion<AgentDbContext, Configuration>());
        }

        public AgentDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        public AgentDbContext(DbConnection dbConnection) : base(dbConnection, true)
        {
            
        }

        public DbSet<Setting> Settings { get; set; }
    }
}