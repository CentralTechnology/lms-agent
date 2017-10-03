namespace Core.Common.Client
{
    using System;
    using Abp;
    using Abp.Extensions;
    using Abp.Threading;
    using Administration;
    using Constants;
    using Helpers;
    using NLog;
    using Simple.OData.Client;

    public class PortalODataClientSettings : ODataClientSettings
    {
        public PortalODataClientSettings()
        {
            BaseUri = new Uri(Constants.DefaultServiceUrl);
           // RenewHttpConnection = false;
           // IgnoreUnmappedProperties = true;
           // RequestTimeout = TimeSpan.FromSeconds(15);
        }

    }
}