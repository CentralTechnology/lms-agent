using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Serilization
{
    [Flags]
    public enum SerializationOptions
    {
        None = 0,
        Exclude = 1,
    }
}
