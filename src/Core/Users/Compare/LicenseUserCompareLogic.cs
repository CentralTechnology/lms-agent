namespace Core.Users.Compare
{
    using System.Collections.Generic;
    using KellermanSoftware.CompareNetObjects;

    public class LicenseUserCompareLogic : CompareLogic
    {
        public LicenseUserCompareLogic()
        {
            Config = new ComparisonConfig
            {
                MembersToIgnore = new List<string> {"Groups", "ManagedSupportId"}
            };
        }
    }
}