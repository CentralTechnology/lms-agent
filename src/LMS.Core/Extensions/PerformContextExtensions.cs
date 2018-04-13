namespace LMS.Core.Extensions
{
    using System;
    using global::Hangfire.Console;
    using global::Hangfire.Server;

    public static class PerformContextExtensions
    {
        private static readonly object ConsoleOutputLock = new object();
        private static readonly object CancelLock = new object();

        public static void Cancel(this PerformContext performContext)
        {
            lock (CancelLock)
            {
                performContext.CancellationToken?.ThrowIfCancellationRequested();
            }
        }

        private static void WriteColorLine(PerformContext performContext, string message, ConsoleTextColor color)
        {
            if (performContext == null)
            {
                return;
            }

            lock (ConsoleOutputLock)
            {
                performContext.SetTextColor(color);
                performContext.WriteLine(message);
                performContext.ResetTextColor();
            }
        }

        public static void WriteErrorLine(this PerformContext performContext, string message)
        {
            WriteColorLine(performContext, message, ConsoleTextColor.Red);
        }

        public static void WriteSpacer(this PerformContext performContext)
        {
            performContext.WriteLine(Environment.NewLine);
        }

        public static void WriteSuccessLine(this PerformContext performContext, string message)
        {
            WriteColorLine(performContext, message, ConsoleTextColor.Green);
        }

        public static void WriteWarnLine(this PerformContext performContext, string message)
        {
            WriteColorLine(performContext, message, ConsoleTextColor.Yellow);
        }
    }
}
