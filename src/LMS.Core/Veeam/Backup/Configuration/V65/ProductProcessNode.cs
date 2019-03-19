using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
    internal class ProductProcessNode : IProductProcessNode
    {
        private readonly IEnumerable<IProductProcess> _processes;

        public IProductProcessNode Parent { get; private set; }

        public IProductProcess Item { get; private set; }

        public ProductProcessNode(
            IEnumerable<IProductProcess> processes,
            IProductProcessNode parent,
            IProductProcess item)
        {
            if (processes == null)
                throw new ArgumentNullException(nameof (processes));
            if (item == null)
                throw new ArgumentNullException(nameof (item));
            this._processes = processes;
            this.Parent = parent;
            this.Item = item;
        }

        public IEnumerable<IProductProcessNode> SubItems
        {
            get
            {
                return (IEnumerable<IProductProcessNode>) this._processes.Where<IProductProcess>((Func<IProductProcess, bool>) (process => ((IEnumerable<ProductProcessOwner>) process.Owners).Any<ProductProcessOwner>((Func<ProductProcessOwner, bool>) (parent =>
                {
                    if (parent.Id == this.Item.Id)
                        return parent.Platform == this.Item.Platform;
                    return false;
                })))).Select<IProductProcess, ProductProcessNode>((Func<IProductProcess, ProductProcessNode>) (process => new ProductProcessNode(this._processes, (IProductProcessNode) this, process)));
            }
        }

        public override string ToString()
        {
            return this.Item.ToString();
        }
    }
}
