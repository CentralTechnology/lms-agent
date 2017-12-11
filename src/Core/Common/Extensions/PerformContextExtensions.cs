namespace LMS.Common.Extensions
{
    using global::Hangfire.Server;

    public static class PerformContextExtensions
    {
        public static void Cancel(this PerformContext performContext)
        {
            performContext?.CancellationToken.ThrowIfCancellationRequested();
        }
    }
}
