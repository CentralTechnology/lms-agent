namespace LMS.Core.Serialization
{
    using System;
    using System.Collections;

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
