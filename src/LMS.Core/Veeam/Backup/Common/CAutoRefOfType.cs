using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
    public class CAutoRef<T> where T : IDisposable
    {
        private T m_detachableObj;

        public CAutoRef(CAutoRefScope scope, T detachableObj)
        {
            if (scope == null)
                throw new ArgumentNullException(nameof (scope));
            this.m_detachableObj = detachableObj;
            scope.Add<T>(detachableObj);
        }

        public bool HasObject
        {
            get
            {
                return (object) this.m_detachableObj != null;
            }
        }

        public T Get()
        {
            return this.m_detachableObj;
        }
    }
}
