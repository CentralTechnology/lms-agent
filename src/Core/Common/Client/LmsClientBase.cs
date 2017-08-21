namespace Core.Common.Client
{
    using NLog;
    using OData;
    using SharpRaven;
    using Simple.OData.Client;

    public class LmsClientBase
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        protected ODataClient Client;
        protected RavenClient RavenClient = Sentry.RavenClient.New();

        public LmsClientBase(ODataCommonClientSettings settings)
        {
            Client = new ODataClient(settings);
        }
    }
}