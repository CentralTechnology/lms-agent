using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseMonitoringSystem.Core.Common.Extensions
{
    using Microsoft.OData.Client;

    public static class DataServiceCollectionExtentions
    {
        public static DataServiceCollection<T> Convert<T>(this IEnumerable<T> source)
        {
            return new DataServiceCollection<T>(source);
        }
    }
}
