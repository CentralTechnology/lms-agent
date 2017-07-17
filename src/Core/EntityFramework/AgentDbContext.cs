namespace Core.EntityFramework
{
    using System.Data.Entity;
    using Administration;
    using Migrations;

    public class AgentDbContext : DbContext
    {
        public AgentDbContext() : base("Default")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<AgentDbContext, Configuration>());
        }

        public DbSet<Setting> Settings { get; set; }
    }
}