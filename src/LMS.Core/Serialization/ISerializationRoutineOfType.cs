using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Serilization
{
    public interface ISerializationRoutine<TSource, TTarget> : ISerializationRoutine
    {
        TTarget ToTarget(TSource target);

        TSource ToSource(TTarget valsourceue);
    }
}
