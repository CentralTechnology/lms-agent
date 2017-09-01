namespace Core.Common.Extensions
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Sockets;
    using System.Security;
    using System.Threading.Tasks;
    using Client.OData;
    using Newtonsoft.Json;
    using NLog;
    using SharpRaven;
    using SharpRaven.Data;
    using Simple.OData.Client;

    public static class ExceptionExtensions
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly RavenClient RavenClient = Sentry.RavenClient.New();

        public static void Handle(this WebRequestException ex, Logger logger)
        {
            try
            {
                var rootException = JsonConvert.DeserializeObject<ODataResponseWrapper>(ex.Response);
                if (rootException != null)
                {
                    InnerError inner = rootException.Error.InnerError;
                    if (inner != null)
                    {
                        logger.Error($"Code: {ex.Code}  Message: {inner.Message}");
                    }
                    else
                    {
                        logger.Error($"Code: {ex.Code}  Message: {rootException.Error.Message}");
                    }
                }

                logger.Debug(ex);
            }
            catch (Exception exc)
            {
                logger.Error(exc);
                logger.Debug(exc);
                throw;
            }
        }

        public static void Handle(this Exception ex)
        {
            if (ex is HttpRequestException httpRequestException)
            {
                Logger.Error("There was a problem communicating with the api.");
                Logger.Debug(httpRequestException);
                return;
            }

            if (ex is SocketException socketException)
            {
                Logger.Error("There was a problem communicating with the api.");
                Logger.Debug(socketException);
                return;
            }

            if (ex is TaskCanceledException taskCanceledException)
            {
                if (taskCanceledException.CancellationToken.IsCancellationRequested)
                {
                    Logger.Error(ex.Message);
                    Logger.Debug(taskCanceledException);
                }
                else
                {
                    Logger.Error("Http request timeout.");
                    Logger.Debug(taskCanceledException);
                }

                return;
            }

            if (ex is WebException webException)
            {
                if (webException.Status == WebExceptionStatus.NameResolutionFailure)
                {
                    Logger.Error(webException.Message);
                    Logger.Debug(webException);
                    return;
                }

                RavenClient.Capture(new SentryEvent(webException));
                Logger.Error(webException.Message);
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

            if (ex is SecurityException securityException)
            {
                Logger.Error(securityException.Message);
                Logger.Debug(securityException);
                return;
            }

            RavenClient.Capture(new SentryEvent(ex));
            Logger.Error(ex.Message);
        }
    }
}