namespace LMS.Core.Managers
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Abp.Domain.Services;
    using Extensions;
    using global::Hangfire.Console;
    using global::Hangfire.Server;
    using Serilog;
    using Services;

    public abstract class WorkerManagerBase : DomainService
    {
        private const string FailedMessage = "Action Failed.";
        private const string StartMessage = "Action Start.";
        private const string SuccessMessage = "Action Success.";

        private readonly ILogger _logger = Log.ForContext<WorkerManagerBase>();

        protected WorkerManagerBase(IPortalService portalService)
        {
            PortalService = portalService;
        }

        public IPortalService PortalService { get; set; }

        protected async Task ExecuteAsync(PerformContext performContext, Func<Task> action)
        {
            try
            {
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
        }

        private void HandleException(PerformContext performContext, Exception ex)
        {
            if (ex is AggregateException aggEx)
            {
                foreach (var innerException in aggEx.Flatten().InnerExceptions)
                {
                    Exception nestedInnerException = innerException;
                    do
                    {
                        if (!string.IsNullOrEmpty(nestedInnerException.Message))
                        {
                            LogException(performContext, nestedInnerException);
                        }

                        nestedInnerException = nestedInnerException.InnerException;
                    } while (nestedInnerException != null);
                }
            }
            else
            {
                LogException(performContext, ex);
            }
        }

        private void LogException(PerformContext performContext, Exception ex)
        {
            _logger.Error(ex.Message);
            _logger.Debug(ex, ex.Message);
            performContext?.WriteErrorLine($"Exception Type: {ex.GetType().Name}");
            performContext?.WriteErrorLine($"Exception Message: {ex.Message}");
        }

        public abstract Task StartAsync(PerformContext performContext);
    }
}