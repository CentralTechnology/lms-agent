namespace LMS.Users.Compare
{
    using System.Collections.Generic;
    using KellermanSoftware.CompareNetObjects;

    public class LicenseGroupCompareLogic : CompareLogic
    {
        public LicenseGroupCompareLogic()
        {
            Config = new ComparisonConfig
            {
                MaxMillisecondsDateDifference = 1000,
                MembersToInclude = new List<string> {"Name", "WhenCreated"}
            };
        }
    }
}