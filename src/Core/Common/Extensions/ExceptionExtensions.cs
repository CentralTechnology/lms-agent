namespace Core.Common.Extensions
{
    using System;
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
                dynamic response = JsonConvert.DeserializeObject(ex.Response);
                if (response != null)
                {
                    logger.Object.Error($"Status: {response.error?.code} \t Message: {response.error?.message}");

                    logger.Object.Debug("", ex);
                }
            }
        }
    }
}