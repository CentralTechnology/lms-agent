using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Serilization
{
    [Flags]
    public enum DescriptorGenerationRules
    {
        None = 0,
        Fields = 1,
        Properties = 2,
    }
}
