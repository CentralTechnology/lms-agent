namespace LMS.Common.Helpers
{
    using System.Collections.Generic;
    using Abp.Logging;
    using global::Hangfire;
    using global::Hangfire.Storage;
    using global::Hangfire.Storage.Monitoring;

    public static class HangfireHelper
    {
        public static void CancelAllJobs()
        {
            IMonitoringApi mon = JobStorage.Current.GetMonitoringApi();
            JobList<ProcessingJobDto> processingJobs = mon.ProcessingJobs(0, int.MaxValue);

            foreach (KeyValuePair<string, ProcessingJobDto> job in processingJobs)
            {               
                BackgroundJob.Delete(job.Key);
                LogHelper.Logger.Info($"Deleted Job {job.Key}");
            }
        }
    }
}