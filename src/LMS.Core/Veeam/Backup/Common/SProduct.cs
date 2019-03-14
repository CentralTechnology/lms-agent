using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Core.Veeam.Backup.Configuration.V65;

namespace LMS.Core.Veeam.Backup.Common
{
    public static class SProduct
    {
        private static IProduct _product = BackupProduct.Create();

        public static IProduct Instance
        {
            get
            {
                if (SProduct._product == null)
                    throw new Exception("Product is not specified.");
                return SProduct._product;
            }
        }

        public static void Init(IProduct product)
        {
            SProduct._product = product;
        }
    }
}
