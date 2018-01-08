namespace LMS.Common.Extensions
{
    using System;
    using Abp.Logging;
    using Castle.Core.Logging;
    using global::Hangfire.Console;
    using global::Hangfire.Server;

    public static class LoggerExtensions
    {
        public static void Debug(this ILogger logger, PerformContext performContext, string message)
        {
            logger.Log(LogSeverity.Debug, performContext, message);
        }

        public static void Debug(this ILogger logger, PerformContext performContext, string message, Exception exception)
        {
            logger.Log(LogSeverity.Debug, performContext, message, exception);
        }

        public static void Error(this ILogger logger, PerformContext performContext, string message)
        {
            logger.Log(LogSeverity.Error, performContext, message);
        }

        public static void Error(this ILogger logger, PerformContext performContext, string message, Exception ex)
        {
            logger.Log(LogSeverity.Error, performContext, message, ex);
        }

        public static void Info(this ILogger logger, PerformContext performContext, string message)
        {
            logger.Log(LogSeverity.Info, performContext, message);
        }

        public static void Log(this ILogger logger, LogSeverity severity, PerformContext performContext, string message)
        {
            logger.Log(severity, message);
            if (performContext == null)
            {
                return;
            }

            switch (severity)
            {
                case LogSeverity.Debug:
                    if (logger.IsDebugEnabled)
                    {
                        performContext.WriteToConsole(ConsoleTextColor.DarkGray, message);
                    }
                    break;
                case LogSeverity.Info:
                    if (logger.IsInfoEnabled)
                    {
                        performContext.WriteToConsole(ConsoleTextColor.Gray, message);
                    }
                    break;
                case LogSeverity.Warn:
                    if (logger.IsWarnEnabled)
                    {
                        performContext.WriteToConsole(ConsoleTextColor.Yellow, message);
                    }
                    break;
                case LogSeverity.Error:
                    if (logger.IsErrorEnabled)
                    {
                        performContext.WriteToConsole(ConsoleTextColor.Red, message);
                    }
                    break;
                case LogSeverity.Fatal:
                    if (logger.IsFatalEnabled)
                    {
                        performContext.WriteToConsole(ConsoleTextColor.Red, message);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(severity), severity, null);
            }
        }

        public static void Log(this ILogger logger, LogSeverity severity, PerformContext performContext, string message, Exception exception)
        {
            logger.Log(severity, message, exception);
            if (performContext == null)
            {
                return;
            }

            switch (severity)
            {
                case LogSeverity.Debug:
                    performContext.SetTextColor(ConsoleTextColor.DarkGray);
                    performContext.WriteLine(message);
                    performContext.ResetTextColor();
                    break;
                case LogSeverity.Info:
                    performContext.SetTextColor(ConsoleTextColor.Gray);
                    performContext.WriteLine(message);
                    performContext.ResetTextColor();
                    break;
                case LogSeverity.Warn:
                    performContext.SetTextColor(ConsoleTextColor.Yellow);
                    performContext.WriteLine(message);
                    performContext.ResetTextColor();
                    break;
                case LogSeverity.Error:
                    performContext.SetTextColor(ConsoleTextColor.Red);
                    performContext.WriteLine(message);
                    performContext.ResetTextColor();
                    break;
                case LogSeverity.Fatal:
                    performContext.SetTextColor(ConsoleTextColor.Red);
                    performContext.WriteLine(message);
                    performContext.ResetTextColor();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(severity), severity, null);
            }
        }

        private static void WriteToConsole(this PerformContext performContext, ConsoleTextColor color, string message)
        {
            performContext.SetTextColor(ConsoleTextColor.DarkGray);
            performContext.WriteLine(message);
            performContext.ResetTextColor();
        }
    }
}