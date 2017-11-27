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
                MembersToIgnore = new List<string> {"Users", "DeletionTime", "IsDeleted"},
                MaxMillisecondsDateDifference = 1000
            };
        }
    }
}