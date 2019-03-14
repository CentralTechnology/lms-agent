namespace LMS.Core.Veeam.Backup.Common
{
    public interface ITransactionProvider
    {
        CTransaction BeginTransaction(CTransactionScopeIdentifier? identifier = null);

        void CommitTransaction(CTransactionScopeIdentifier identifier);

        void RollbackTransaction(CTransactionScopeIdentifier identifier);
    }
}
