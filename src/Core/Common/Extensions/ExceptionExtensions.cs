namespace Core.Common.Extensions
{
    using Abp.Dependency;
    using Castle.Core.Logging;
    using Client;
    using Client.OData;
    using Newtonsoft.Json;
    using Simple.OData.Client;

    public static class ExceptionExtensions
    {
        public static void FormatWebRequestException(this WebRequestException ex)
        {           
            using (var logger = IocManager.Instance.ResolveAsDisposable<ILogger>())
            {
                try
                {
                    ODataResponseWrapper response = JsonConvert.DeserializeObject<ODataResponseWrapper>(ex.Response);
                    if (response != null)
                    {
                        logger.Object.Error($"Status: {response.Error.Code}");
                        logger.Object.Error($"Message: {response.Error.Message}");

                        if (response.Error.InnerError != null)
                        {
                            logger.Object.Error($"Inner Message: {response.Error.InnerError.Message}");
                        }
                    }
                }
                catch (JsonReaderException jre)
                {
                    logger.Object.Debug("Unable to parse WebRequestException to ODataResponseWrapper");
                    logger.Object.Debug(jre.ToString);
                }
                finally
                {
                    logger.Object.Debug(ex.ToString);
                }               
            }
        }
    }
}