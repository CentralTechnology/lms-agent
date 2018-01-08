namespace LMS.Veeam.Mappings
{
    interface IPerVmStoredProceduresMapping
    {
        string CanProcessVm { get; }

        string GetPerVmRestorePointsData { get; }
        string GetProtectedVms { get; }

        string GetVmsNumbers { get; }
    }
}