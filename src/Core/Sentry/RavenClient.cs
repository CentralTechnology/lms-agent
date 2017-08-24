namespace Core.Sentry
{
    using Common.Constants;
    using Common.Helpers;

    public static class RavenClient
    {
        public static SharpRaven.RavenClient New()
        {
            var client = new SharpRaven.RavenClient(Constants.SentryDSN)
            {
                Environment = DebuggingService.Debug ? "development" : "production",
                Release = AppVersionHelper.Version
            };

            return client;
        }
    }
}