using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Serilization
{
    internal sealed class ArrayTypeDescriptor : IListTypeDescriptor
    {
        public ArrayTypeDescriptor(Type type, TypeDescriptor elementType)
            : base(type, SerializationRoute.Array, elementType)
        {
        }

        public override IList CreateList(int capacity)
        {
            return (IList) Array.CreateInstance(this.Child.Type, capacity);
        }

        public override void SetValue(IList list, int index, object value)
        {
            list[index] = value;
        }

        public override void CommitList(IList list, int itemCount)
        {
        }
    }
}
