namespace LMS.Migrations
{
    using System.Data.Entity.Migrations;
    using EntityFramework;

    internal sealed  class Configuration : DbMigrationsConfiguration<LMSDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationDataLossAllowed = false;
            AutomaticMigrationsEnabled = true;
            ContextKey = "LMS";
        }
    }
}
