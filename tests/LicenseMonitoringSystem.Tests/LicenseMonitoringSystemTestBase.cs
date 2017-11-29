namespace LicenseMonitoringSystem.Tests
{
    using System;
    using System.Threading.Tasks;
    using Abp.WebApi.Client;
    using LMS.Common.Client;
    using LMS.Common.Helpers;
    using LMS.EntityFramework;
    using NSubstitute;
    using Ploeh.AutoFixture;

    public abstract class LicenseMonitoringSystemTestBase
    {
     
        protected AbpWebApiClient AbpWebApiClient; 
        protected PortalWebApiClient PortalWebApiClient; 
        protected Fixture Fixture = new Fixture();
        
        protected LicenseMonitoringSystemTestBase()
        {
            AbpWebApiClient = Substitute.For<AbpWebApiClient>();
            PortalWebApiClient = Substitute.For<PortalWebApiClient>(AbpWebApiClient);
           





            AbpWebApiClient.PostAsync<string>(Fixture.Create<string>()).Returns(Fixture.Create<string>());
            PortalWebApiClient.GetAntiForgeryToken().Returns(Fixture.Create<string>());
            //SettingManagerHelper.Instance.AccountId.Returns(Fixture.Create<int>());
            //SettingManagerHelper.Instance.DeviceId.Returns(Fixture.Create<Guid>());
            //SettingManagerHelper.Instance.Token.Returns(Fixture.Create<Guid>().ToString());

        }



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