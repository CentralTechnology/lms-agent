namespace LMS.Users.Compare
{
    using System.Collections.Generic;
    using KellermanSoftware.CompareNetObjects;

    public class LicenseUserCompareLogic : CompareLogic
    {
        public LicenseUserCompareLogic()
        {
            Config = new ComparisonConfig
            {
                MaxMillisecondsDateDifference = 1000,
                MembersToInclude = new List<string> {"DisplayName", "Email", "Enabled", "FirstName", "LastLoginDate", "SamAccountName", "Surname", "WhenCreated"}
            };
        }
    }
}