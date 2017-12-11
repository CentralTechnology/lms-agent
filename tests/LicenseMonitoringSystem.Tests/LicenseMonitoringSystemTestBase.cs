namespace LicenseMonitoringSystem.Tests
{
    using System;
    using System.Data.Common;
    using System.Threading.Tasks;
    using Abp.TestBase;
    using Castle.MicroKernel.Registration;
    using LMS.EntityFramework;
    using Ploeh.AutoFixture;

    public abstract class LicenseMonitoringSystemTestBase : AbpIntegratedTestBase<LicenseMonitoringSystemTestModule>
    {

        protected override void PreInitialize()
        {
            //Fake DbConnection using Effort!
            LocalIocManager.IocContainer.Register(
                Component.For<DbConnection>()
                    .UsingFactoryMethod(Effort.DbConnectionFactory.CreateTransient)
                    .LifestyleSingleton()
            );

            base.PreInitialize();
        }




        #region UsingDbContext

        protected void UsingDbContext(string connectionString, Action<LMSDbContext> action)
        {
            using (var context = LocalIocManager.Resolve<LMSDbContext>())
            {
                action(context);
                context.SaveChanges();
            }
        }

        protected async Task UsingDbContextAsync(Func<LMSDbContext, Task> action)
        {
            using (var context = LocalIocManager.Resolve<LMSDbContext>())
            {
                await action(context);
                await context.SaveChangesAsync();
            }
        }

        protected T UsingDbContext<T>(Func<LMSDbContext, T> func)
        {
            T result;

            using (var context = LocalIocManager.Resolve<LMSDbContext>())
            {
                result = func(context);
                context.SaveChanges();
            }

            return result;
        }

        protected async Task<T> UsingDbContextAsync<T>(Func<LMSDbContext, Task<T>> func)
        {
            T result;

            using (var context = LocalIocManager.Resolve<LMSDbContext>())
            {
                result = await func(context);
                await context.SaveChangesAsync();
            }

            return result;
        }

        #endregion
    }
}