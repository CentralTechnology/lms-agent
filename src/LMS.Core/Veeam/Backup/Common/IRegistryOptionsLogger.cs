using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
    public interface IRegistryOptionsLogger : IDisposable
    {
        void Message(string format, params object[] args);

        void Warning(string format, params object[] args);

        void Error(string format, params object[] args);

        void Exception(Exception exception, string format, params object[] args);
    }
}
