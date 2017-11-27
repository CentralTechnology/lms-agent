namespace LMS.Common.Extensions
{
    using SharpRaven;

    public static class RavenClientExtensions
    {
        public static void AddTag(this RavenClient ravenClient, string key, string value)
        {
            bool keyExists = ravenClient.Tags.TryGetValue(key, out string existingValue);
            if (!keyExists)
            {
                ravenClient.Tags.Add(key, value);
            }
        }
    }
}