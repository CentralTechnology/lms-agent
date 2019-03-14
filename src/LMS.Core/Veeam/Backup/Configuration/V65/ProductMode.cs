using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
    internal class ProductMode
    {
        public static bool IsPreview(ProductReleaseAttributes attributes, IProduct product)
        {
            if (attributes == null)
                throw new ArgumentNullException(nameof (attributes));
            if (product == null)
                throw new ArgumentNullException(nameof (product));
            if (!attributes.Preview)
                return false;
            using (IRegistryConfigurationController registryController = product.CreateRegistryController(false, RegistryView.Default))
            {
                if (registryController.IsReady)
                    return registryController.GetValue("EnablePreviewMode", true);
            }
            return true;
        }

        private static class Registry
        {
            public const string EnablePreviewMode = "EnablePreviewMode";
        }
    }
}
