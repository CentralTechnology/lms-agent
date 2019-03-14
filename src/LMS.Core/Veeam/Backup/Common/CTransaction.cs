using System;

namespace LMS.Core.Veeam.Backup.Common
{
    public class CTransaction : IDisposable
    {
        private readonly Guid _id;
        private readonly CTransactionScopeIdentifier _identifier;
        private readonly ITransactionProvider _transactionProvider;
        private bool _completed;

        public CTransaction(
            CTransactionScopeIdentifier identifier,
            ITransactionProvider transactionProvider)
        {
            Exceptions.CheckArgumentNullException<ITransactionProvider>(transactionProvider, nameof (transactionProvider));
            this._id = Guid.NewGuid();
            this._identifier = identifier;
            this._transactionProvider = transactionProvider;
        }

        public Guid Id
        {
            get
            {
                return this._id;
            }
        }

        public CTransactionScopeIdentifier Identifier
        {
            get
            {
                return this._identifier;
            }
        }

        public ITransactionProvider TransactionProvider
        {
            get
            {
                return this._transactionProvider;
            }
        }

        public bool Completed
        {
            get
            {
                return this._completed;
            }
        }

        public void SetExactAbortOn(IDatabaseAccessor dbAccessor)
        {
            dbAccessor.ExecCommand(this.Identifier, "SET XACT_ABORT ON;");
        }

        public void Dispose()
        {
            this.Rollback();
        }

        public void Commit()
        {
            if (this._completed)
                return;
            this._completed = true;
            this._transactionProvider.CommitTransaction(this.Identifier);
        }

        public void Rollback()
        {
            if (this._completed)
                return;
            this._completed = true;
            this._transactionProvider.RollbackTransaction(this.Identifier);
        }
    }
}
