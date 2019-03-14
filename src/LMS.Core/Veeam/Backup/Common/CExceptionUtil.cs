using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Security.Authentication;
using System.Text;
using LMS.Core.Common;
using Serilog;

namespace LMS.Core.Veeam.Backup.Common
{
  public static class CExceptionUtil
  {
    public static Exception GetFirstChanceExc(Exception ex)
    {
      Stack<Exception> exceptions = new Stack<Exception>();
      Stack<AggregateException> aggregateExceptions = new Stack<AggregateException>();
      CExceptionUtil.ExpandExceptions(ex, exceptions, aggregateExceptions);
      if (aggregateExceptions.Count <= 0)
        return exceptions.Pop();
      return (Exception) aggregateExceptions.Pop();
    }

    public static T FindInitialException<T>(Exception ex) where T : Exception
    {
      Stack<Exception> exceptionStack = new Stack<Exception>();
      Stack<AggregateException> aggregateExceptions = new Stack<AggregateException>();
      CExceptionUtil.ExpandExceptions(ex, exceptionStack, aggregateExceptions);
      if (aggregateExceptions.Count > 0)
      {
        T obj = aggregateExceptions.Pop() as T;
        if ((object) obj != null)
          return obj;
      }
      return exceptionStack.OfType<T>().FirstOrDefault<T>();
    }

    public static IEnumerable<T> GetAllOfType<T>(Exception ex) where T : Exception
    {
      Stack<Exception> exceptionStack = new Stack<Exception>();
      Stack<AggregateException> aggregateExceptions = new Stack<AggregateException>();
      CExceptionUtil.ExpandExceptions(ex, exceptionStack, aggregateExceptions);
      return exceptionStack.OfType<T>();
    }

    public static void ExpandExceptions(
      Exception ex,
      Stack<Exception> exceptions,
      Stack<AggregateException> aggregateExceptions)
    {
      for (; ex != null; ex = ex.InnerException)
      {
        AggregateException aggregateException = ex as AggregateException;
        if (aggregateException != null)
        {
          aggregateExceptions.Push(aggregateException);
          using (IEnumerator<Exception> enumerator = aggregateException.InnerExceptions.GetEnumerator())
          {
            while (enumerator.MoveNext())
              CExceptionUtil.ExpandExceptions(enumerator.Current, exceptions, aggregateExceptions);
            break;
          }
        }
        else
          exceptions.Push(ex);
      }
    }

    public static Exception GetRealException(Exception givenExc)
    {
      Exception exception = givenExc;
      while (exception is CRegeneratedTraceException || exception is CRegeneratedUserException || (exception is TargetInvocationException || exception is CSatelliteCriticalFaultException))
        exception = exception.InnerException;
      return exception;
    }

    public static bool IsCredsException(Exception exception)
    {
      if (exception == null)
        return false;
      if (exception is InvalidCredentialException)
        return true;
      return CExceptionUtil.GetRealException(exception) is InvalidCredentialException;
    }

    public static bool IsRemotingException(Exception givenExc)
    {
      if (givenExc == null)
        return false;
      Exception realException = CExceptionUtil.GetRealException(givenExc);
      return realException is CMethodReconnectException || realException is CMethodRetryException || (realException is RemotingException || realException is AuthenticationException) || (realException is SocketException || realException is CVbrServiceMaintainceException || realException is IOException && realException.InnerException is SocketException);
    }

    public static bool IsSatelliteException(Exception givenExc)
    {
      return CExceptionUtil.GetRealException(givenExc) is CSatelliteCriticalFaultException;
    }

    public static bool IsRegenerated(Exception givenExc)
    {
      return givenExc.InnerException != null;
    }

    [DebuggerStepThrough]
    public static void RegenTraceExc(
      Exception originalExc,
      string formatString,
      params object[] args)
    {
      throw new CRegeneratedTraceException(originalExc, formatString, args);
    }

    [DebuggerStepThrough]
    public static void RegenUserExc(
      Exception originalExc,
      string formatString,
      params object[] args)
    {
      throw new CRegeneratedUserException(originalExc, formatString, args);
    }

    public static bool IsTerminatingOrCanceling(Exception exc)
    {
      Exception firstChanceExc = CExceptionUtil.GetFirstChanceExc(exc);
      if (!(firstChanceExc is TerminatedException))
        return firstChanceExc is OperationCanceledException;
      return true;
    }

    public static bool IsTerminating(Exception exc)
    {
      return CExceptionUtil.GetFirstChanceExc(exc) is TerminatedException;
    }

    public static void ThrowArgumentException(string format, params object[] args)
    {
      throw new ArgumentException(string.Format(format, args));
    }

    public static Exception RebuildCppStyleException(Exception originalExc)
    {
      return CExceptionUtil.RebuildCppStyleException(originalExc.Message);
    }

    public static Exception RebuildCppStyleException(string originalExcMessage)
    {
      string[] strArray = originalExcMessage.Split(new string[1]
      {
        "\n"
      }, StringSplitOptions.RemoveEmptyEntries);
      CCppComponentExceptionBuilder exceptionBuilder = CCppComponentExceptionBuilder.Create();
      foreach (string line in strArray)
        exceptionBuilder.AddMessage(line);
      return exceptionBuilder.GenerateResult();
    }

    public static Exception RebuildRpcException(
      Exception originalExc,
      string exceptionMessage)
    {
      string[] strArray = originalExc.Message.Split(new string[1]
      {
        "\n"
      }, StringSplitOptions.RemoveEmptyEntries);
      COMException comException = originalExc as COMException;
      int errorCode;
      if (comException != null)
      {
        errorCode = comException.ErrorCode;
      }
      else
      {
        errorCode = 0;
        Log.Warning("Unexpected rpc exception type {0}", (object) originalExc.GetType());
      }
      CCppComponentExceptionBuilder forComException = CCppComponentExceptionBuilder.CreateForComException(errorCode, exceptionMessage);
      foreach (string line in strArray)
        forComException.AddMessage(line);
      return forComException.GenerateResult();
    }

    public static string GetOriginalRpcErrorMessage(Exception e)
    {
      StringBuilder stringBuilder = new StringBuilder();
      string message = e.Message;
      string[] separator = new string[1]{ "\n" };
      foreach (string str1 in message.Split(separator, StringSplitOptions.RemoveEmptyEntries))
      {
        char[] chArray = new char[4]
        {
          '\r',
          '\n',
          '\t',
          ' '
        };
        string str2 = str1.Trim(chArray);
        if (!str2.StartsWith("--tr:", StringComparison.InvariantCultureIgnoreCase))
        {
          if (stringBuilder.ToString().Length != 0 && !stringBuilder.ToString().EndsWith(". "))
          {
            if (stringBuilder.ToString().EndsWith("."))
              stringBuilder.Append(" ");
            else
              stringBuilder.Append(". ");
          }
          stringBuilder.Append(str2);
        }
      }
      if (stringBuilder.ToString().Length != 0 && !stringBuilder.ToString().EndsWith("."))
        stringBuilder.Append(".");
      return stringBuilder.ToString();
    }

    public static Exception TryRecreateOriginalRpcException(Exception ex)
    {
      if (ex == null)
        return (Exception) null;
      Exception realException = CExceptionUtil.GetRealException(ex);
      if (realException == null)
        return ex;
      return ExceptionFactory.Create(CExceptionUtil.GetOriginalRpcErrorMessage(realException));
    }

    public static string RemoveTraceMessages(Exception givenExc)
    {
      StringBuilder stringBuilder = new StringBuilder();
      string message = givenExc.Message;
      string[] separator = new string[2]{ "\r\n", "\n" };
      foreach (string str in message.Split(separator, StringSplitOptions.RemoveEmptyEntries))
      {
        if (str.Length <= 5 || string.Compare(str.Substring(0, 5), "--tr:", StringComparison.InvariantCulture) != 0)
          stringBuilder.AppendLine(str);
      }
      return stringBuilder.ToString();
    }

    public static string GetSqlUnavalibleMessage(Exception innerException)
    {
      return string.Format("Microsoft SQL server hosting the configuration database is currently unavailable. Possible reasons are heavy load, networking issue, server reboot, or hot backup.{0}Please wait, and try again later.{0}{0}Error information:{0}{1}", (object) Environment.NewLine, (object) innerException.FormatMessage(false));
    }

    public static string GetServiceTimeoutMessage(Exception innerException, string servicename)
    {
      return string.Format("{0} is currently unavailable. Possible reasons are heavy load, networking issue, server reboot, or hot backup.{1}Please wait, and try again later.{1}{1}Error information:{1}{2}", (object) servicename, (object) Environment.NewLine, (object) innerException.FormatMessage(false));
    }

    public static void ThrowSqlException(Exception innerException)
    {
      Log.Error(innerException, (string) null);
      throw new CSqlException(innerException, CExceptionUtil.GetSqlUnavalibleMessage(innerException), new object[0]);
    }

    public static void ThrowSqlException(Exception innerException, bool retyable)
    {
      if (!retyable)
        Log.Error(innerException, (string) null);
      throw new CSqlException(innerException, CExceptionUtil.GetSqlUnavalibleMessage(innerException), new object[0]);
    }

    public static TObject FindDataValue<TObject>(Exception exc)
    {
      Exception exception = exc;
      while (exception != null)
      {
        TObject @object = exception.Data.Values.OfType<TObject>().FirstOrDefault<TObject>();
        if ((object) @object != null)
          return @object;
        AggregateException aggregateException = exception as AggregateException;
        if (aggregateException != null)
        {
          foreach (Exception innerException in aggregateException.InnerExceptions)
          {
            TObject dataValue = CExceptionUtil.FindDataValue<TObject>(innerException);
            if ((object) dataValue != null)
              return dataValue;
          }
          exception = (Exception) null;
        }
        else
          exception = exception.InnerException;
      }
      return default (TObject);
    }

    public static TObject FindDataValue<TObject>(Exception exc, object key)
    {
      Exception exception = exc;
      while (exception != null)
      {
        if (exception.Data.Contains(key) && exception.Data[key] is TObject)
          return (TObject) exception.Data[key];
        AggregateException aggregateException = exception as AggregateException;
        if (aggregateException != null)
        {
          foreach (Exception innerException in aggregateException.InnerExceptions)
          {
            TObject dataValue = CExceptionUtil.FindDataValue<TObject>(innerException, key);
            if ((object) dataValue != null)
              return dataValue;
          }
          exception = (Exception) null;
        }
        else
          exception = exception.InnerException;
      }
      return default (TObject);
    }

    public static string GetAllMessages(Exception e)
    {
      string str = string.Empty;
      Dictionary<string, int> dictionary = new Dictionary<string, int>();
      for (; e != null; e = e.InnerException)
      {
        if (!dictionary.ContainsKey(e.Message))
        {
          if (str.Length == 0)
            str = e.Message;
          else
            str = e.Message.Trim(' ', '\r', '\n', '\t') + "\n\n" + str;
          dictionary[e.Message] = 1;
        }
      }
      return str;
    }

    public static bool HasException<T>(Exception givenExc)
    {
      for (Exception exception = givenExc; exception != null; exception = exception.InnerException)
      {
        if (exception is T)
          return true;
      }
      return false;
    }

    public static bool MessageContains(Exception e, string text)
    {
      for (; e != null; e = e.InnerException)
      {
        if (e.Message.ToLowerInvariant().Contains(text.ToLowerInvariant()))
          return true;
      }
      return false;
    }

    public static Exception GetFirstNonAggregateException(Exception exc)
    {
      Exception firstChanceExc = CExceptionUtil.GetFirstChanceExc(exc);
      AggregateException aggregateException = firstChanceExc as AggregateException;
      if (aggregateException != null)
        return aggregateException.InnerExceptions[0];
      return firstChanceExc;
    }
  }
}
