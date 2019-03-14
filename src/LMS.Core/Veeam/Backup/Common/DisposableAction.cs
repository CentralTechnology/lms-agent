using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace LMS.Core.Veeam.Backup.Common
{
    public sealed class DisposableAction : IDisposable
    {
        private readonly Action _onDispose;
        private bool _isCanceled;
        private readonly bool _safe;
        private bool _isDisposed;

        public DisposableAction(Action onDispose)
        {
            this._onDispose = onDispose;
            this._isDisposed = false;
        }

        public DisposableAction(Action onDispose, bool safe)
        {
            this._onDispose = onDispose;
            this._safe = safe;
            this._isDisposed = false;
        }

        public void Dispose()
        {
            if (this._isCanceled)
                return;
            if (this._isDisposed)
                return;
            try
            {
                this._onDispose.NullSafeInvoke();
            }
            catch (Exception ex)
            {
                if (!this._safe)
                    throw new Exception("[DisposableAction] Failed to invoke.", ex);
                Log.Error(ex, "[DisposableAction] Failed to invoke.");
            }
            finally
            {
                this._isDisposed = true;
            }
        }

        public void Cancel()
        {
            this._isCanceled = true;
        }
    }
}
