namespace LMS.Core.Veeam.Backup.Common
{
    [System.Flags]
    public enum ExceptionTraversingOptions
    {
        None = 0,
        DontReturnAggregateException = 1,
        DontExpanAggregateException = 2,
    }
}
