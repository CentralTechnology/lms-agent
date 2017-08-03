namespace LicenseMonitoringSystem.Tests
{
    using System;
    using System.Threading.Tasks;
    using global::Core.EntityFramework;

    public abstract class LicenseMonitoringSystemTestBase
    {
        #region UsingDbContext

        protected void UsingDbContext(string connectionString, Action<AgentDbContext> action)
        {
            using (var context = new AgentDbContext(connectionString))
            {
                action(context);
                context.SaveChanges();
            }
        }

        protected async Task UsingDbContextAsync(Func<AgentDbContext, Task> action)
        {
            using (var context = new AgentDbContext())
            {
                await action(context);
                await context.SaveChangesAsync();
            }
        }

        protected T UsingDbContext<T>(Func<AgentDbContext, T> func)
        {
            T result;

            using (var context = new AgentDbContext())
            {
                result = func(context);
                context.SaveChanges();
            }

            return result;
        }

        protected async Task<T> UsingDbContextAsync<T>(Func<AgentDbContext, Task<T>> func)
        {
            T result;

            using (var context = new AgentDbContext())
            {
                result = await func(context);
                await context.SaveChangesAsync();
            }

            return result;
        }

        #endregion
    }
}