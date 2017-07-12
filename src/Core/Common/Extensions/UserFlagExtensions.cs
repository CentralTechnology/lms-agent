namespace Core.Common.Extensions
{
    using Models;

    public static class UserFlagExtensions
    {
        /// <summary>
        ///     Check if flags contains the specific user flag. This method is more efficient compared to 'HasFlag()'.
        /// </summary>
        /// <param name="haystack">The bunch of flags</param>
        /// <param name="needle">The flag to look for.</param>
        /// <returns>Return true if flag found in flags.</returns>
        public static bool Contains(this UserFlags haystack, UserFlags needle)
        {
            return (haystack & needle) == needle;
        }
    }
}