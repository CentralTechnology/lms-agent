namespace LMS.Common.Managers
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using Abp;
    using Extensions;
    using global::Hangfire.Console;
    using global::Hangfire.Server;
    using Interfaces;

    public abstract class WorkerManagerBase : LMSManagerBase, IWorkerManager
    {
        private const string FailedMessage = "Action Failed.";
        private const string StartMessage = "Action Start.";
        private const string SuccessMessage = "Action Success.";
        public abstract void Start(PerformContext performContext);

        public static bool InProgress { get; private set; }

        protected void SetJobRunning(bool status)
        {
            InProgress = status;
        }

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
                throw;
            }
            finally
            {
                SetJobRunning(false);
            }
        }

        protected void HandleException(PerformContext performContext, Exception ex)
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

        protected void HandleExceptionInternal(PerformContext performContext, Exception ex)
        {
            switch (ex)
            {
                case AbpException abp:
                    Logger.Error(performContext, abp.Message);
                    Logger.Debug(performContext, abp.Message, abp);
                    throw abp;

                case JobAbortedException jobAborted:
                    Logger.Error(performContext, jobAborted.Message);
                    Logger.Debug(performContext, jobAborted.Message, jobAborted);
                    throw jobAborted;

                case OperationCanceledException operationCancelled:
                    Logger.Error(performContext, operationCancelled.Message);
                    Logger.Debug(performContext, operationCancelled.Message, operationCancelled);
                    throw operationCancelled;

                case WebException web:
                    Logger.Error(performContext, web.Message, web);
                    throw web;

                default:
                    RavenClient.Capture(new SharpRaven.Data.SentryEvent(ex));
                    Logger.Error(performContext, ex.Message);
                    Logger.Debug(performContext, ex.Message, ex);
                    throw ex;
            }
        }
    }
}