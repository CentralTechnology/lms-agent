using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
    public static class ActionExm
    {
        public static void NullSafeInvoke(this Action action)
        {
            if (action == null)
                return;
            action();
        }

        public static void NullSafeInvoke<T>(this Action<T> action, T value)
        {
            if (action == null)
                return;
            action(value);
        }

        public static void NullSafeInvoke<T>(this EventHandler<T> action, object sender, T value) where T : EventArgs
        {
            if (action == null)
                return;
            action(sender, value);
        }

        public static void NullSafeInvoke<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            if (action == null)
                return;
            action(arg1, arg2);
        }

        public static T NullSafeInvokeAndReturnArg<T>(this Action<T> action, T value)
        {
            action.NullSafeInvoke<T>(value);
            return value;
        }
    }
}
