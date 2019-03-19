using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
    public interface IEnumeratorEx<out T> : IEnumerator<T>, IDisposable, IEnumerator
    {
        bool MovePrevious();

        int Index { get; set; }
    }
}
