namespace LicenseMonitoringSystem.Tests
{
    using System;
    using System.Threading.Tasks;
    using Abp.WebApi.Client;
    using global::Core.Administration;
    using global::Core.EntityFramework;
    using LMS.Common.Client;
    using LMS.Common.Helpers;
    using NSubstitute;
    using Ploeh.AutoFixture;

    public abstract class LicenseMonitoringSystemTestBase
    {
        protected SettingManagerHelper SettingManagerHelper = Substitute.For<SettingManagerHelper>();       
        protected AbpWebApiClient AbpWebApiClient; 
        protected PortalWebApiClient PortalWebApiClient; 
        protected Fixture Fixture = new Fixture();
        
        protected LicenseMonitoringSystemTestBase()
        {
            SettingManagerHelper.SetTestingInstance(SettingManagerHelper);
            AbpWebApiClient = Substitute.For<AbpWebApiClient>();
            PortalWebApiClient = Substitute.For<PortalWebApiClient>(AbpWebApiClient);
           





            AbpWebApiClient.PostAsync<string>(Fixture.Create<string>()).Returns(Fixture.Create<string>());
            PortalWebApiClient.GetAntiForgeryToken().Returns(Fixture.Create<string>());
            //SettingManagerHelper.Instance.AccountId.Returns(Fixture.Create<int>());
            //SettingManagerHelper.Instance.DeviceId.Returns(Fixture.Create<Guid>());
            //SettingManagerHelper.Instance.Token.Returns(Fixture.Create<Guid>().ToString());

        }



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