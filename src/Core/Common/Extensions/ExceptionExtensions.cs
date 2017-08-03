using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Extensions
{
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
                    var inner = rootException.Error.InnerError;
                    if (inner != null)
                    {
                        logger.Error($"Code: {ex.Code}");
                        logger.Error($"Message: {inner.Message}");
                    }
                    else
                    {
                        logger.Error($"Code: {ex.Code}");
                        logger.Error($"Message: {rootException.Error.Message}");
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Error(exc);
                throw;
            }
        }
    }
}
