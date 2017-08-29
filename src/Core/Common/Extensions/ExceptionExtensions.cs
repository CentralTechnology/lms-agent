namespace Core.Common.Extensions
{
    using System;
    using Client.OData;
    using Newtonsoft.Json;
    using NLog;
    using Simple.OData.Client;

    public static class ExceptionExtensions
    {
        public static void Handle(this WebRequestException ex, Logger logger)
        {
            try
            {
                var rootException = JsonConvert.DeserializeObject<ODataResponseWrapper>(ex.Response);
                if (rootException != null)
                {
                    InnerError inner = rootException.Error.InnerError;
                    if (inner != null)
                    {
                        logger.Error($"Code: {ex.Code}  Message: {inner.Message}");
                    }
                    else
                    {
                        logger.Error($"Code: {ex.Code}  Message: {rootException.Error.Message}");
                    }
                }

                logger.Debug(ex);
            }
            catch (Exception exc)
            {
                logger.Error(exc);
                logger.Debug(exc);
                throw;
            }
        }
    }
}