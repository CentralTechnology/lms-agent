using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class COptionsScopeAttribute : System.Attribute
    {
        public string Prefix { get; private set; }

        public COptionsScopeAttribute(string prefix)
        {
            this.Prefix = prefix;
        }
    }
}
