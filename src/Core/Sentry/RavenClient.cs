namespace Core.Sentry
{
    using Common.Constants;
    using Common.Helpers;

    public static class RavenClient
    {
        static RavenClient()
        {
            
        }

        public static SharpRaven.RavenClient Instance { get; } = new SharpRaven.RavenClient(Constants.SentryDSN)
        {
            Environment = DebuggingService.Debug ? "development" : "production",
            Release = AppVersionHelper.Version
        };
    }
}