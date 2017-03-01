using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Common.Extensions
{
    using Castle.Core.Logging;
    using ShellProgressBar;

    public static class ProgressBarExtensions
    {
        public static void Tick(this ProgressBar pbar,string message, LoggerLevel loggerLevel)
        {
            pbar.Tick(message);
            ConsoleExtensions.WriteLineBottom(message, loggerLevel);
        }

        public static void Tick(this ChildProgressBar childProgressBar, string message, LoggerLevel loggerLevel)
        {
            childProgressBar.Tick(message);
            ConsoleExtensions.WriteLineBottom(message, loggerLevel);
        }

        public static void UpdateMessage(this ProgressBar pbar, string message, LoggerLevel loggerLevel)
        {
            pbar.UpdateMessage(message);
            ConsoleExtensions.WriteLineBottom(message, loggerLevel);
        }

        public static void UpdateMessage(this ChildProgressBar childProgressBar, string message, LoggerLevel loggerLevel)
        {
            childProgressBar.UpdateMessage(message);
            ConsoleExtensions.WriteLineBottom(message, loggerLevel);
        }
    }
}
