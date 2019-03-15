using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
    public class ProductServerName
    {
        public string Name { get; set; }

        public string InstanceName { get; set; }

        public ProductServerName(string name, string instanceName)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof (name));
            this.Name = name;
            this.InstanceName = instanceName;
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(this.InstanceName))
                return this.Name;
            return string.Format("{0}\\{1}", (object) this.Name, (object) this.InstanceName);
        }

        public static implicit operator string(ProductServerName name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof (name));
            return name.ToString();
        }

        public static bool TryParse(string fullName, out ProductServerName name)
        {
            name = (ProductServerName) null;
            if (string.IsNullOrEmpty(fullName))
                return false;
            string[] strArray = fullName.Split('\\');
            switch (strArray.Length)
            {
                case 1:
                    name = new ProductServerName(strArray[0], string.Empty);
                    return true;
                case 2:
                    name = new ProductServerName(strArray[0], strArray[1]);
                    return true;
                default:
                    return false;
            }
        }

        public static ProductServerName Parse(string fullName)
        {
            ProductServerName name;
            if (!ProductServerName.TryParse(fullName, out name))
                throw new ArgumentException(nameof (fullName));
            return name;
        }
    }
}
