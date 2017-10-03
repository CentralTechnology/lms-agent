namespace Core.Common.Client
{
    using System;
    using System.Threading.Tasks;
    using Administration;
    using NLog;
    using OData;
    using Polly;
    using SharpRaven;
    using Simple.OData.Client;

    public class PortalODataClientBase
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        protected ODataClient Client;
        protected Policy DefaultPolicy;
        protected RavenClient RavenClient;
        protected SettingManager SettingManager;

        public PortalODataClientBase()
        {
            Client = PortalODataClient.New();
            RavenClient = Sentry.RavenClient.Instance;
            SettingManager = new SettingManager();

            DefaultPolicy = Policy
                .Handle<TaskCanceledException>()
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(15),
                    TimeSpan.FromSeconds(30)
                }, (exception, timeSpan, retryCount, context) => { Logger.Error($"Retry {retryCount} of {context.PolicyKey} at {context.ExecutionKey}, due to: {exception.Message}."); });
        }
    }
}