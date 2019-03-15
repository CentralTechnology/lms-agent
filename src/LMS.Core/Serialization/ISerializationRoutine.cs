namespace LMS.Core.Serialization
{
    public interface ISerializationRoutine
    {
        object ToTarget(object target);

        object ToSource(object source);
    }
}
