namespace LMS.Veeam.Mappings
{
    internal class CPerVmTrialStoredProceduresMapping : IPerVmStoredProceduresMapping
    {
        public string GetProtectedVms => "[dbo].[GetProtectedVms.Trial]";

        public string GetVmsNumbers => "[dbo].[GetVmsNumbers.Trial]";

        public string CanProcessVm => "[dbo].[CanProcessVm.Trial]";

        public string GetPerVmRestorePointsData => "[dbo].[GetPerVmRestorePointsData.Trial]";
    }
}