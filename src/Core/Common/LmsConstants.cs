namespace Core.Common
{
    public class LmsConstants
    {
        /// <summary>
        /// </summary>
        public const string LoggerTarget = "colouredConsole|file";

#if DEBUG

        /// <summary>
        ///     Base url for the api client
        /// </summary>
        public const string BaseServiceUrl = "http://localhost:61814/";

#else /// <summary>
/// Base url for the api client
/// </summary>  
        public const string BaseServiceUrl = "https://portal.ct.co.uk";
#endif

#if DEBUG
        /// <summary>
        ///     Default endpoint for OData
        /// </summary>
        public const string DefaultServiceUrl = "http://localhost:61814/odata/v1";
#else /// <summary>
///     Default endpoint for OData
/// </summary>
        public const string DefaultServiceUrl = "https://portal.ct.co.uk/odata/v1";

#endif
        /// <summary>
        /// </summary>

        /// <summary>
        ///     Doesn't actually provide any admin access. Its just used to hide menu items.
        /// </summary>
        public const string AdminAccess = "a490d701-b267-4d4a-87bc-fe0d0f4c9105";
    }
}