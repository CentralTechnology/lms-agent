namespace LMS.Core.Serialization
{
    public interface ISerializationRoutine<TSource, TTarget> : ISerializationRoutine
    {
        TTarget ToTarget(TSource target);

        TSource ToSource(TTarget valsourceue);
    }
}
