namespace LMS.Core.Extensions
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
                case LogSeverity.Info:
                    performContext?.WriteLine(message);
                    break;
                case LogSeverity.Warn:
                    performContext?.WriteWarnLine(message);
                    break;
                case LogSeverity.Error:
                    performContext?.WriteErrorLine(message);
                    break;
                default:
                    performContext?.WriteLine(message);
                    break;
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
                case LogSeverity.Info:
                    performContext?.WriteLine(message);
                    break;
                case LogSeverity.Warn:
                    performContext?.WriteWarnLine(message);
                    break;
                case LogSeverity.Error:
                    performContext?.WriteErrorLine(message);
                    break;
                default:
                    performContext?.WriteLine(message);
                    break;
            }
        }
    }
}