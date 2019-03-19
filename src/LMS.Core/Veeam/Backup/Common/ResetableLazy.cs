using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
    public sealed class ResetableLazy<T>
    {
        private readonly object _lock = new object();
        private readonly Func<T> _valueFactory;
        private readonly Action<T> _valueChecker;
        private Lazy<T> _lazy;

        public ResetableLazy(Func<T> valueFactory)
        {
            this._valueFactory = valueFactory;
            this.Reset();
        }

        public ResetableLazy(Func<T> valueFactory, Action<T> valueChecker)
            : this(valueFactory)
        {
            this._valueChecker = valueChecker;
        }

        public void Reset()
        {
            lock (this._lock)
            {
                if (this._lazy != null && this.IsValueCreated)
                    ((object) this.Value as IDisposable).SafeDispose();
                this._lazy = new Lazy<T>(this._valueFactory, true);
            }
        }

        public T Value
        {
            get
            {
                lock (this._lock)
                    return this._lazy.Value;
            }
            set
            {
                if (this._valueChecker != null)
                    this._valueChecker(value);
                lock (this._lock)
                    this._lazy = new Lazy<T>((Func<T>) (() => value), true);
            }
        }

        public bool IsValueCreated
        {
            get
            {
                lock (this._lock)
                    return this._lazy.IsValueCreated;
            }
            set
            {
                lock (this._lock)
                {
                    if (this._lazy.IsValueCreated == value)
                        return;
                    if (value)
                    {
                        if ((object) this._lazy.Value == null)
                        {
                        }
                    }
                    else
                        this.Reset();
                }
            }
        }
    }
}
