namespace LicenseMonitoringSystem.Core.Common.Extensions
{
    using System.Collections.Generic;
    using Microsoft.OData.Client;

    public static class DataServiceCollectionExtentions
    {
        public static DataServiceCollection<T> Convert<T>(this IEnumerable<T> source)
        {
            return new DataServiceCollection<T>(source);
        }
    }
}