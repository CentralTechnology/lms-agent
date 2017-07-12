namespace Core.Common.Extensions
{
    using Abp.Dependency;
    using Castle.Core.Logging;
    using Newtonsoft.Json;
    using Simple.OData.Client;

    public static class ExceptionExtensions
    {
        public static void HandleWebRequestException(WebRequestException ex)
        {
            using (IDisposableDependencyObjectWrapper<ILogger> logger = IocManager.Instance.ResolveAsDisposable<ILogger>())
            {
                bool valid = ex.Response.IsValidJson();
                if (!valid)
                {
                    logger.Object.Error(ex.Message);
                    logger.Object.Debug(ex.ToString());
                    return;
                }

                dynamic response = JsonConvert.DeserializeObject(ex.Response);
                if (response != null)
                {
                    logger.Object.Error($"Status: {response.error?.code} \t Message: {response.error?.message}");

                    logger.Object.Debug("Error during WebRequest", ex);
                }
            }
        }
    }
}