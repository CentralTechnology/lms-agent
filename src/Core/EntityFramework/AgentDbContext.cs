using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityFramework
{
    using System.Data.Entity;
    using Administration;
    using Migrations;

    public class AgentDbContext : DbContext
    {
        public AgentDbContext() : base("Default")
        {
            Database.SetInitializer<AgentDbContext>(new MigrateDatabaseToLatestVersion<AgentDbContext, Configuration>());
        }

        public DbSet<Setting> Settings { get; set; }
    }
}
