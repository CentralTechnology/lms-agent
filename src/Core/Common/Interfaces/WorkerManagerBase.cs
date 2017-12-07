using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Common.Interfaces
{
    using System.Diagnostics;
    using System.Net;
    using Abp;
    using Abp.Logging;
    using Extensions;
    using global::Hangfire.Console;
    using global::Hangfire.Server;
    using Managers;

    public abstract class WorkerManagerBase : LMSManagerBase, IWorkerManager
    {
        private const string StartMessage = "Action Start.";
        private const string SuccessMessage = "Action Success.";
        private const string FailedMessage = "Action Failed.";
        public abstract void Start(PerformContext performContext);

        protected void Execute(PerformContext performContext, Action action)
        {
            try
            {
                performContext?.WriteLine(StartMessage);
                Stopwatch stopwatch = Stopwatch.StartNew();

                action.Invoke();

                stopwatch.Stop();
                Logger.Log(LogSeverity.Info, performContext, SuccessMessage);
                Logger.Log(LogSeverity.Info, performContext, $"Action took {stopwatch.Elapsed}");
            }
            catch (Exception ex)
            {
                HandleException(performContext, ex);
                Logger.Log(LogSeverity.Error, performContext, FailedMessage);
                throw;
            }
        }

        protected void HandleException(PerformContext performContext, Exception ex)
        {
            if (ex is AggregateException aggex)
            {
                foreach (var innerException in aggex.InnerExceptions)
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
                    Logger.Log(LogSeverity.Error, performContext, abp.Message, abp);
                    throw abp;

                case JobAbortedException jobAborted:
                    Logger.Log(LogSeverity.Error, performContext, jobAborted.Message, jobAborted);
                    throw jobAborted;

                case OperationCanceledException operationCancelled:
                    Logger.Log(LogSeverity.Error, performContext, operationCancelled.Message, operationCancelled);
                    throw operationCancelled;

                case WebException web:
                    Logger.Log(LogSeverity.Error, performContext, web.Message, web);
                    throw web;

                default:
                    RavenClient.Capture(new SharpRaven.Data.SentryEvent(ex));
                    Logger.Log(LogSeverity.Error, performContext, ex.Message, ex);
                    break;
            }


        }
    }
}
