namespace Core.Common.Extensions
{
    using System;
    using Abp.Dependency;
    using Castle.Core.Logging;

    public class ConsoleExtensions
    {
        public static void WriteLineBottom(string text, LoggerLevel loggerLevel)
        {
            int x = Console.CursorLeft;
            int y = Console.CursorTop;
            Console.CursorTop = Console.WindowTop + Console.WindowHeight - 1;

            using (var logger = IocManager.Instance.ResolveAsDisposable<ILogger>())
            {
                switch (loggerLevel)
                {
                    case LoggerLevel.Off:
                        break;
                    case LoggerLevel.Fatal:
                        logger.Object.Fatal(text);
                        break;
                    case LoggerLevel.Error:
                        logger.Object.Error(text);
                        break;
                    case LoggerLevel.Warn:
                        logger.Object.Warn(text);
                        break;
                    case LoggerLevel.Info:
                        Console.WriteLine(text);
                        break;
                    case LoggerLevel.Debug:
                        logger.Object.Debug(text);
                        break;
                }
            }

            // Restore previous position
            Console.SetCursorPosition(x, y);
        }
    }
}