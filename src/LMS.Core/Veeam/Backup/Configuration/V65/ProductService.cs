using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
    internal class ProductService : ProductProcess, IProductService, IProductProcess
    {
        public int Order { get; private set; }

        public string ServiceName { get; private set; }

        public bool CanBeDisabled { get; private set; }

        public ProductService(
            Guid id,
            ProcessPlatform platform,
            ProcessType type,
            string name,
            string fileName,
            int order,
            string serviceName,
            bool canBeDisabled,
            string productCode,
            string componentId,
            string description)
            : base(id, platform, type, name, fileName, new ProductProcessOwner[0], productCode, componentId, description)
        {
            if (order < 0)
                throw new ArgumentOutOfRangeException(nameof (order));
            if (string.IsNullOrEmpty(serviceName))
                throw new ArgumentNullException(nameof (serviceName));
            this.Order = order;
            this.ServiceName = serviceName;
            this.CanBeDisabled = canBeDisabled;
        }
    }
}
