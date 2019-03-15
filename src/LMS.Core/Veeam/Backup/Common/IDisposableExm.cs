using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace LMS.Core.Veeam.Backup.Common
{
    public static class IDisposableExm
    {
        private const string ErrorFmt = "Failed to dispose \"{0}\"{1}Full callstack:{1}{2}";

        public static void SafeDispose(this IDisposable self)
        {
            try
            {
                if (object.ReferenceEquals((object) self, (object) null))
                    return;
                self.Dispose();
            }
            catch (ObjectDisposedException ex)
            {
                StackTrace stackTrace = new StackTrace(true);
                Log.Error( (Exception) ex, "Failed to dispose \"{0}\"{1}Full callstack:{1}{2}", (object) self, (object) Environment.NewLine, (object) stackTrace.ToString());
            }
            catch (Exception ex)
            {
                StackTrace stackTrace = new StackTrace(true);
                Log.Error(ex, "Failed to dispose \"{0}\"{1}Full callstack:{1}{2}", (object) self, (object) Environment.NewLine, (object) stackTrace.ToString());
            }
        }

        public static void SafeDispose<T>(this Lazy<T> self) where T : IDisposable
        {
            if (object.ReferenceEquals((object) self, (object) null) || !self.IsValueCreated)
                return;
            self.Value.SafeDispose();
        }

        public static void SafeDispose<T>(this ResetableLazy<T> self) where T : IDisposable
        {
            if (object.ReferenceEquals((object) self, (object) null) || !self.IsValueCreated)
                return;
            self.Value.SafeDispose();
        }

        public static void SafeDispose(this IEnumerable<IDisposable> self)
        {
            if (object.ReferenceEquals((object) self, (object) null))
                return;
            foreach (IDisposable self1 in self)
                self1.SafeDispose();
        }
    }
}
