using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class COptionsTimeSpanAttribute : COptionsValueAttribute
    {
        public long DefaultValue { get; set; }

        public EOptionsTimeSpanFormat From { get; set; }

        public COptionsTimeSpanAttribute(long defaultValue, EOptionsTimeSpanFormat from)
        {
            this.DefaultValue = defaultValue;
            this.From = from;
        }
    }
}
