namespace Core.Common.Client
{
    using Administration;
    using NLog;
    using OData;
    using SharpRaven;
    using Simple.OData.Client;

    public class LmsClientBase
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        protected ODataClient Client;
        protected RavenClient RavenClient = Sentry.RavenClient.New();
        protected SettingManager SettingManager = new SettingManager();

        public LmsClientBase(ODataCommonClientSettings settings)
        {
            Client = new ODataClient(settings);
        }
    }
}