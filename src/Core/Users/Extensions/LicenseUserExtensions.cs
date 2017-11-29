namespace LMS.Users.Extensions
{
    using Abp.Extensions;
    using Portal.LicenseMonitoringSystem.Users.Entities;

    public static class LicenseUserExtensions
    {
        public static string Format(this LicenseUser licenseUser, bool debug = false)
        {
            string userText;
            if (!licenseUser.DisplayName.IsNullOrEmpty())
            {
                userText = licenseUser.DisplayName;
            }
            else if (!licenseUser.FirstName.IsNullOrEmpty() && !licenseUser.Surname.IsNullOrEmpty())
            {
                userText = $"{licenseUser.FirstName} {licenseUser.Surname}";
            }
            else
            {
                userText = licenseUser.SamAccountName;
            }

            return userText + (debug ? $" - {licenseUser.Id}" : string.Empty);
        }
    }
}