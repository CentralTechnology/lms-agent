using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
    public class CAutoRefScope : IDisposable
    {
        private CDisposableList m_disposableObjects = new CDisposableList(true);
        private bool m_isObjectsAreDetached;

        public T Add<T>(T detachableObj) where T : IDisposable
        {
            return this.m_disposableObjects.Add<T>(detachableObj);
        }

        public IDisposable DetachAndCommit()
        {
            this.Commit();
            return (IDisposable) this.m_disposableObjects;
        }

        public void Commit()
        {
            this.m_isObjectsAreDetached = true;
        }

        public T Commit<T>(T detachableObjectsOwner) where T : IDisposable
        {
            this.Commit();
            return detachableObjectsOwner;
        }

        public static void ExecGuarded(Action<CAutoRefScope> a)
        {
            using (CAutoRefScope cautoRefScope = new CAutoRefScope())
            {
                a(cautoRefScope);
                cautoRefScope.Commit();
            }
        }

        public static T ExecGuarded<T>(Func<CAutoRefScope, T> f) where T : IDisposable
        {
            using (CAutoRefScope cautoRefScope = new CAutoRefScope())
                return cautoRefScope.Commit<T>(f(cautoRefScope));
        }

        void IDisposable.Dispose()
        {
            if (this.m_isObjectsAreDetached)
                return;
            this.m_disposableObjects.Dispose();
        }
    }
}
