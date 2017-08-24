namespace Core.Common.Helpers
{
    using System;
    using System.IO;
    using Abp.Extensions;
    using Extensions;

    /// <summary>
    ///     Central point for application version.
    /// </summary>
    public class AppVersionHelper
    {
        private static string _version;

        /// <summary>
        ///     Gets release (last build) date of the application.
        ///     It's shown in the web page.
        /// </summary>
        public static DateTime ReleaseDate
        {
            get
            {
                string file = typeof(AppVersionHelper).GetAssembly().Location;
                return file == null ? default(DateTime) : new FileInfo(file).LastWriteTime;
            }
        }

        /// <summary>
        ///     Gets current version of the application.
        ///     It's also shown in the web page.
        /// </summary>
        public static string Version
        {
            get
            {
                if (_version.IsNullOrEmpty())
                {
                    string version = typeof(AppVersionHelper).Assembly.GetName().Version.ToString();
                    if (version.IsNullOrEmpty())
                    {
                        return "1.0.0";
                    }

                    _version = version;
                    return version;
                }

                return _version;
            }
        }
    }
}