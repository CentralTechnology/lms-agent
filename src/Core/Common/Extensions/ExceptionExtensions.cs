namespace Core.Common.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Sockets;
    using System.Security;
    using System.Text;
    using Microsoft.OData.Client;
    using NLog;
    using SharpRaven;
    using SharpRaven.Data;

    public static class ExceptionExtensions
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly RavenClient RavenClient = Sentry.RavenClient.Instance;

        public static string GetFullMessage(this Exception exception)
        {
            //return ex.InnerException == null
            //    ? ex.Message
            //    : ex.Message + " --> " + ex.InnerException.GetFullMessage();

            //if (ex == null)
            //{
            //    return string.Empty;
            //}

            //if (messages == "")
            //{
            //    return ex.Message;
            //}

            //var sb = new StringBuilder(messages);
            //if (ex.InnerException != null)
            //{
            //    sb.AppendLine($"\r\nInnerException: {GetFullMessage(ex.InnerException)}");
            //}

            //return sb.ToString();

            var messages = exception.FromHierarchy(ex => ex.InnerException)
                .Select(ex => ex.Message);
            return String.Join(Environment.NewLine, messages);
        }

        public static IEnumerable<TSource> FromHierarchy<TSource>(
            this TSource source,
            Func<TSource, TSource> nextItem,
            Func<TSource, bool> canContinue)
        {
            for (var current = source; canContinue(current); current = nextItem(current))
            {
                yield return current;
            }
        }

        public static IEnumerable<TSource> FromHierarchy<TSource>(
            this TSource source,
            Func<TSource, TSource> nextItem)
            where TSource : class
        {
            return FromHierarchy(source, nextItem, s => s != null);
        }

        public static void Handle(this Exception ex)
        {
            if (ex is DataServiceClientException
                || ex is SqlException
                || ex is HttpRequestException
                || ex is SocketException
                || ex is WebException
                || ex is SecurityException)
            {
                Logger.Error(ex.GetFullMessage());
                Logger.Debug(ex.ToString());
                return;
            }

            if (ex is IOException ioException)
            {
                // chances are the reason this is thrown is because the client is low on disk space.
                // therefore we output to console instead of the logger.

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ioException.Message);
                Console.ResetColor();
                return;
            }

            Logger.Error(ex.Message);
            Logger.Debug(ex);
            RavenClient.Capture(new SentryEvent(ex));
        }
    }
}