namespace LMS.Core.Extensions.Managers
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Threading.Tasks;
    using Abp.UI;
    using Extensions;
    using Core.Common.Managers;
    using Core.Services;
    using Core.Services.Authentication;
    using global::Hangfire.Console;
    using global::Hangfire.Server;
    using Microsoft.OData.Client;

    public abstract class WorkerManagerBase : LMSManagerBase
    {
        private const string FailedMessage = "Action Failed.";
        private const string StartMessage = "Action Start.";
        private const string SuccessMessage = "Action Success.";

        protected WorkerManagerBase(IPortalService portalService, IPortalAuthenticationService authService)
        {
            PortalService = portalService;
            AuthService = authService;
        }

        public IPortalAuthenticationService AuthService { get; set; }

        private static bool InProgress { get; set; }

        public IPortalService PortalService { get; set; }

        protected void Execute(PerformContext performContext, Action action)
        {
            try
            {
                SetJobRunning(true);

                performContext?.WriteLine(StartMessage);
                Stopwatch stopwatch = Stopwatch.StartNew();

                action.Invoke();

                stopwatch.Stop();
                performContext?.SetTextColor(ConsoleTextColor.Green);
                performContext?.WriteLine(SuccessMessage);
                Logger.Info(performContext, $"Action took {stopwatch.Elapsed}");
            }
            catch (Exception ex)
            {
                HandleException(performContext, ex);
                Logger.Error(performContext, FailedMessage);
                throw; // throw to fail the background job
            }
            finally
            {
                SetJobRunning(false);
            }
        }

        protected async Task ExecuteAsync(PerformContext performContext, Func<Task> action)
        {
            try
            {
                SetJobRunning(true);

                performContext?.WriteLine(StartMessage);
                Stopwatch stopwatch = Stopwatch.StartNew();

                await action.Invoke();

                stopwatch.Stop();
                performContext?.WriteSuccessLine(SuccessMessage);
                performContext?.WriteSuccessLine($"Action took {stopwatch.Elapsed}");
            }
            catch (Exception ex)
            {
                HandleException(performContext, ex);
                performContext?.WriteErrorLine(FailedMessage);
                throw;
            }
            finally
            {
                SetJobRunning(false);
            }
        }

        private void HandleException(PerformContext performContext, Exception ex)
        {
            if (ex is AggregateException aggex)
            {
                foreach (Exception innerException in aggex.InnerExceptions)
                {
                    HandleExceptionInternal(performContext, innerException);
                }
            }
            else
            {
                HandleExceptionInternal(performContext, ex);
            }
        }

        private void HandleExceptionInternal(PerformContext performContext, Exception ex)
        {
            switch (ex)
            {
                case DataServiceClientException dataServiceClient:
                    Logger.Error(performContext, dataServiceClient.Message);
                    Logger.Debug(performContext, dataServiceClient.Message, dataServiceClient);
                    break;

                case DataServiceRequestException dataServiceRequest:
                    Logger.Error(performContext, dataServiceRequest.Message);
                    Logger.Debug(performContext, dataServiceRequest.Message, dataServiceRequest);
                    break;

                case JobAbortedException jobAborted:
                    Logger.Error(performContext, jobAborted.Message);
                    Logger.Debug(performContext, jobAborted.Message, jobAborted);
                    break;

                case OperationCanceledException operationCancelled:
                    Logger.Error(performContext, operationCancelled.Message);
                    Logger.Debug(performContext, operationCancelled.Message, operationCancelled);
                    break;

                case OutOfMemoryException outOfMemory:
                    Logger.Error(performContext, outOfMemory.Message);
                    Logger.Debug(performContext, outOfMemory.Message, outOfMemory);
                    break;

                case UserFriendlyException userFriendly:
                    Logger.Error(performContext, userFriendly.Message);
                    Logger.Debug(performContext, userFriendly.Message, userFriendly);
                    break;

                case WebException web:
                    Logger.Error(performContext, web.Message, web);
                    Logger.Debug(performContext, web.Message, web);
                    break;

                default:
                    RavenClient.Capture(new SharpRaven.Data.SentryEvent(ex));
                    Logger.Error(performContext, ex.Message);
                    Logger.Debug(performContext, ex.Message, ex);
                    break;
            }
        }

        private static void SetJobRunning(bool status)
        {
            InProgress = status;
        }

        public abstract Task StartAsync(PerformContext performContext);
    }
}