using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Serilization
{
    internal abstract class IListTypeDescriptor : TypeDescriptor
    {
        protected IListTypeDescriptor(
            Type type,
            SerializationRoute route,
            params TypeDescriptor[] childs)
            : base(type, route, childs)
        {
        }

        public abstract IList CreateList(int capacity);

        public abstract void SetValue(IList list, int index, object value);

        public abstract void CommitList(IList list, int itemCount);
    }
}
