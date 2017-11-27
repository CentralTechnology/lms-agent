namespace LMS.EntityFramework
{
    using System.Data.Common;
    using System.Data.Entity;
    using Abp.BackgroundJobs;
    using Abp.EntityFramework;
    using Abp.Localization;
    using Abp.Notifications;
    using Authorization.Roles;
    using Authorization.Users;
    using Core.Administration;
    using MultiTenancy;

    public class LMSDbContext : AbpDbContext
    {
        /// <summary>
        /// Settings.
        /// </summary>
        public virtual IDbSet<Setting> Settings { get; set; }

        public LMSDbContext()
            : base("Default")
        {

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

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<NotificationInfo>().Property(e => e.Data).HasMaxLength(4000);
        //    modelBuilder.Entity<NotificationInfo>().Property(e => e.TenantIds).HasMaxLength(4000);
        //    modelBuilder.Entity<NotificationInfo>().Property(e => e.UserIds).HasMaxLength(4000);
        //    modelBuilder.Entity<NotificationInfo>().Property(e => e.ExcludedUserIds).HasMaxLength(4000);

        //    modelBuilder.Entity<TenantNotificationInfo>().Property(e => e.Data).HasMaxLength(4000);

        //    modelBuilder.Entity<ApplicationLanguageText>().Property(e => e.Value).HasMaxLength(4000);

        //    modelBuilder.Entity<BackgroundJobInfo>().Property(e => e.JobArgs).HasMaxLength(4000);
        //}
    }
}
