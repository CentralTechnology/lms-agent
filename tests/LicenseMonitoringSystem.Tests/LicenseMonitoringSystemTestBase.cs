namespace LicenseMonitoringSystem.Tests
{
    using System;
    using System.Threading.Tasks;
    using LMS.EntityFramework;
    using Ploeh.AutoFixture;

    public abstract class LicenseMonitoringSystemTestBase
    {
        protected Fixture Fixture = new Fixture();

        #region UsingDbContext

        protected void UsingDbContext(string connectionString, Action<LMSDbContext> action)
        {
            using (var context = new LMSDbContext(connectionString))
            {
                action(context);
                context.SaveChanges();
            }
        }

        protected async Task UsingDbContextAsync(Func<LMSDbContext, Task> action)
        {
            using (var context = new LMSDbContext())
            {
                await action(context);
                await context.SaveChangesAsync();
            }
        }

        protected T UsingDbContext<T>(Func<LMSDbContext, T> func)
        {
            T result;

            using (var context = new LMSDbContext())
            {
                result = func(context);
                context.SaveChanges();
            }

            return result;
        }

        protected async Task<T> UsingDbContextAsync<T>(Func<LMSDbContext, Task<T>> func)
        {
            T result;

            using (var context = new LMSDbContext())
            {
                result = await func(context);
                await context.SaveChangesAsync();
            }

            return result;
        }

        #endregion
    }
}