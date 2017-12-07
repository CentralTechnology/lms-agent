using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Common.Extensions
{
    using Abp.Logging;
    using Castle.Core.Logging;
    using global::Hangfire.Console;
    using global::Hangfire.Server;

    public static class LoggerExtensions
    {
        public static void Log(this ILogger logger, LogSeverity severity, PerformContext performContext, string message)
        {
            Abp.Logging.LoggerExtensions.Log(logger,severity,message);
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

        public static void Log(this ILogger logger, LogSeverity severity, PerformContext performContext, string message, Exception exception)
        {
            Abp.Logging.LoggerExtensions.Log(logger, severity, message);
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
    }
}
