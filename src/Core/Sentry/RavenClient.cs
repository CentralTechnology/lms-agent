namespace Core.Sentry
{
    using Common.Constants;
    using Common.Helpers;

    public sealed class RavenClient
    {
        static RavenClient()
        {
            
        }

        private RavenClient()
        {
            
        }

        public static SharpRaven.RavenClient Instance { get; } = new SharpRaven.RavenClient(Constants.SentryDSN)
        {
            Environment = DebuggingService.Debug ? "development" : "production",
            Release = AppVersionHelper.Version
        };
    }
}