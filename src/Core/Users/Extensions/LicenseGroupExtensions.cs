using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Users.Extensions
{
    using Abp.Extensions;
    using Portal.LicenseMonitoringSystem.Users.Entities;

    public static class LicenseGroupExtensions
    {
        public static string Format(this LicenseGroup licenseGroup, bool debug = false)
        {
            string userText;
            if (!licenseGroup.Name.IsNullOrEmpty())
            {
                userText = licenseGroup.Name;
            }
            else
            {
                userText = licenseGroup.Id.ToString();
            }

            return userText + (debug ? $" - {licenseGroup.Id}" : string.Empty);
        }
    }
}
