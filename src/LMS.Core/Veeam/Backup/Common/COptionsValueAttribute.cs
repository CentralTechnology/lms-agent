using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class COptionsValueAttribute : System.Attribute
    {
        public string FullName { get; set; }

        public string RelativeName { get; set; }

        public string ResolveOptionName(string prefix, string memberName)
        {
            if (this.FullName != null)
                return this.FullName;
            if (this.RelativeName != null)
                return prefix + this.RelativeName;
            return prefix + memberName;
        }
    }
}
