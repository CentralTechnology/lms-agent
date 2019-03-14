using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Serilization
{
    internal interface IExpressionAccessor
    {
        object Get(object instance);

        void Set(ref object instance, object value);
    }
}
