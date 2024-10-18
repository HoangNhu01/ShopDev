using Hangfire;
using Hangfire.Storage;
using Hangfire.Storage.Monitoring;
using ShopDev.Constants.Hangfire;

namespace ShopDev.Utils.Hangfire
{
    public static class HangfireUltils
    {
        public static string SetBackGroundStatus(
            this IStorageConnection connection,
            string backgroundJobId
        )
        {
            JobData jobData = connection.GetJobData(backgroundJobId);
            return jobData is not null ? jobData.State : string.Empty;
        }

        public static int GetRetryCount(string jobId)
        {
            var monitoringApi = JobStorage.Current.GetMonitoringApi();
            var jobDetails = monitoringApi.JobDetails(jobId);
            int retryCount = 0;

            if (jobDetails is not null)
            {
                // Lấy số lần retry
                foreach (var state in jobDetails.History)
                {
                    if (state.StateName == StateNames.Succeeded)
                    {
                        retryCount++;
                    }
                }
            }
            return retryCount;
        }
    }
}
