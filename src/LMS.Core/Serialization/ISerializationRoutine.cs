using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Serilization
{
    public interface ISerializationRoutine
    {
        object ToTarget(object target);

        object ToSource(object source);
    }
}
