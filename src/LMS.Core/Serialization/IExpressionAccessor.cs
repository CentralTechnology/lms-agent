namespace LMS.Core.Serialization
{
    internal interface IExpressionAccessor
    {
        object Get(object instance);

        void Set(ref object instance, object value);
    }
}
