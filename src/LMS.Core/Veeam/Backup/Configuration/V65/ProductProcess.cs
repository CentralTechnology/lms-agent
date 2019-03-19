using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
    internal class ProductProcess : IProductProcess
    {
        public Guid Id { get; private set; }

        public ProcessPlatform Platform { get; private set; }

        public ProcessType Type { get; private set; }

        public string Name { get; private set; }

        public string FileName { get; private set; }

        public ProductProcessOwner[] Owners { get; private set; }

        public string ProductCode { get; private set; }

        public string ComponentId { get; private set; }

        public string Description { get; private set; }

        public ProductProcess(
            Guid id,
            ProcessPlatform platform,
            ProcessType type,
            string name,
            string fileName,
            ProductProcessOwner[] owners,
            string productCode,
            string componentId,
            string description)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof (name));
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof (fileName));
            if (owners == null)
                throw new ArgumentNullException(nameof (owners));
            this.Id = id;
            this.Platform = platform;
            this.Type = type;
            this.Name = name;
            this.FileName = fileName;
            this.Owners = owners;
            this.ProductCode = productCode;
            this.ComponentId = componentId;
            this.Description = description;
        }

        public override string ToString()
        {
            return this.FileName;
        }
    }
}
