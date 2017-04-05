namespace Core.Common.Extensions
{
    using Abp.Dependency;
    using Client;
    using Newtonsoft.Json;
    using NLog;
    using Simple.OData.Client;

    public static class ExceptionExtensions
    {
        public static void FormatWebRequestException(this WebRequestException ex)
        {
            ODataResponseWrapper response = JsonConvert.DeserializeObject<ODataResponseWrapper>(ex.Response);
            using (IDisposableDependencyObjectWrapper<ILogger> logger = IocManager.Instance.ResolveAsDisposable<ILogger>())
            {
                if (response != null)
                {
                    logger.Object.Error($"Status: {response.Error.Code}");
                    logger.Object.Error($"Message: {response.Error.Message}");

                    if (response.Error.InnerError != null)
                    {
                        logger.Object.Error($"Inner Message: {response.Error.InnerError.Message}");
                    }
                }

                logger.Object.Debug(ex.ToString);
            }
        }
    }
}