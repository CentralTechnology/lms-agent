using System;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;

namespace LMS.Core.Veeam.Backup.Common
{
    public static class SExceptionExtensions
    {
        public static Exception GetRealException(this Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof (exception));
            return CExceptionUtil.GetRealException(exception);
        }

        public static Exception GetFirstChanceException(this Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException(nameof (exception));
            return CExceptionUtil.GetFirstChanceExc(exception);
        }

        public static Exception Rethrow(this Exception exception)
        {
            exception.Dispatch().Throw();
            return exception;
        }

        public static ExceptionDispatchInfo Dispatch(this Exception exception)
        {
            return ExceptionDispatchInfo.Capture(exception);
        }

        public static string FormatMessage(this Exception ex, bool withStack = false)
        {
            StringBuilder stringBuilder = new StringBuilder();
            do
            {
                if (SExceptionExtensions.IsToDisplay(ex))
                {
                    if (stringBuilder.Length > 0)
                        stringBuilder.Append(Environment.NewLine);
                    if (string.IsNullOrEmpty(ex.Message))
                    {
                        if (ex is COMException)
                        {
                            COMException comException = (COMException) ex;
                            stringBuilder.Append(Marshal.GetExceptionForHR(comException.ErrorCode).Message);
                        }
                    }
                    else
                        stringBuilder.Append(ex.Message);
                    if (withStack && ex.StackTrace != null)
                        stringBuilder.Append(ex.StackTrace);
                }
                ex = ex.InnerException;
            }
            while (ex != null);
            return stringBuilder.ToString();
        }

        private static bool IsToDisplay(Exception ex)
        {
            return !(ex is TargetInvocationException);
        }
    }
}
