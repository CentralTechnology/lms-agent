namespace LMS.Core.Extensions.Helpers
{
    using System;
    using System.Reflection;

    public class SettingManagerHelper
    {
        public static string ClientVersion
        {
            get
            {
                try
                {
                    return Assembly.GetEntryAssembly().GetName().Version.ToString();
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }
    }
}