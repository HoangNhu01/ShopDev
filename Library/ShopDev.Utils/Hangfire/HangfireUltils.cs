using Hangfire.Storage;

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
    }
}
