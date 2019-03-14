using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Serilization
{
    internal sealed class ListTypeDescriptor : IListTypeDescriptor
    {
        private readonly ConstructorAccessor<int> _constructorAccessor;

        public ListTypeDescriptor(Type type, TypeDescriptor elementType)
            : base(type, SerializationRoute.List, elementType)
        {
            this._constructorAccessor = ListTypeDescriptor.CreateConstructor(type);
        }

        public override IList CreateList(int capacity)
        {
            return (IList) this._constructorAccessor.Create(capacity);
        }

        public override void SetValue(IList list, int index, object value)
        {
            while (list.Count < index)
                list.Add((object) null);
            if (list.Count > index)
                list[index] = value;
            else
                list.Add(value);
        }

        public override void CommitList(IList list, int itemCount)
        {
            while (list.Count < itemCount)
                list.Add((object) null);
        }

        private static ConstructorAccessor<int> CreateConstructor(Type type)
        {
            ConstructorInfo constructor = type.GetConstructor(new Type[1]
            {
                ReflectionTypes.Int32
            });
            if (constructor == (ConstructorInfo) null)
                throw new NotSupportedException("Cannot find an expected constructor: public List(Int32 capacity).");
            ParameterInfo parameter = constructor.GetParameters()[0];
            if (parameter.Name != "capacity")
                throw new NotSupportedException("An unexpected constructor occured: public List(Int32 " + parameter.Name + ").");
            return new ConstructorAccessor<int>(constructor);
        }
    }
}
