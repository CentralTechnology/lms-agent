using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml;
using LMS.Core.Common;

namespace LMS.Core.Veeam.Backup.Common
{
  public static class Exceptions
  {
    [DebuggerStepThrough]
    public static T CheckArgumentNullException<T>(T arg, string argName) where T : class
    {
      if (argName == null)
        throw new ArgumentNullException(nameof (argName));
      if ((object) arg == null)
        throw new ArgumentNullException(argName);
      return arg;
    }

    [DebuggerStepThrough]
    public static void CheckArgumentNullException(params object[] argsAndNames)
    {
      if (argsAndNames.Length % 2 != 0)
        throw new ArgumentException("Invalid argument input.");
      for (int index = 0; index < argsAndNames.Length; index += 2)
      {
        if (argsAndNames[index] == null)
        {
          string argsAndName = argsAndNames[index + 1] as string;
          if (argsAndName == null)
            throw new ArgumentNullException("Invalid argument name.");
          throw new ArgumentNullException(argsAndName);
        }
      }
    }

    [DebuggerStepThrough]
    public static string CheckArgumentNullOrEmptyException(string arg, string argName)
    {
      if (argName == null)
        throw new ArgumentNullException(nameof (argName));
      if (arg == null)
        throw new ArgumentNullException(argName);
      if (arg == string.Empty)
        throw new ArgumentEmptyException(argName);
      return arg;
    }

    public static void Assert(bool condition, string message)
    {
      if (!condition)
        throw new Exception(message);
    }

    [DebuggerStepThrough]
    public static string CheckArgumentEmptyException(string arg, string argName)
    {
      if (argName == null)
        throw new ArgumentNullException(nameof (argName));
      if (arg == string.Empty)
        throw new ArgumentEmptyException(argName);
      return arg;
    }

    [DebuggerStepThrough]
    public static Guid CheckArgumentEmptyException(Guid arg, string argName)
    {
      if (argName == null)
        throw new ArgumentNullException(nameof (argName));
      if (arg == Guid.Empty)
        throw new ArgumentEmptyException(argName);
      return arg;
    }

    [DebuggerStepThrough]
    public static T CheckArgumentNullOrCastException<T>(object arg, string argName)
    {
      if (argName == null)
        throw new ArgumentNullException(nameof (argName));
      if (arg == null)
        throw new ArgumentNullException(argName);
      try
      {
        return (T) arg;
      }
      catch (Exception ex)
      {
        throw new ArgumentException(argName, ex);
      }
    }

    [DebuggerStepThrough]
    public static IEnumerable<T> CheckArgumentNullOrEmptySequenceException<T>(
      IEnumerable<T> arg,
      string argName)
    {
      if (arg == null)
        throw new ArgumentNullException(argName);
      if (!arg.Any<T>())
        throw new ArgumentEmptyException(argName);
      return arg;
    }

    [DebuggerStepThrough]
    public static void CheckArgumentNullOrEmptyException(params string[] argsAndNames)
    {
      if (argsAndNames.Length % 2 != 0)
        throw new ArgumentException("Invalid argument input.");
      for (int index = 0; index < argsAndNames.Length; index += 2)
      {
        if (argsAndNames[index] == null)
          throw new ArgumentNullException(argsAndNames[index + 1]);
        if (argsAndNames[index] == string.Empty)
          throw new ArgumentEmptyException(argsAndNames[index + 1]);
      }
    }

    [DebuggerStepThrough]
    public static T CheckArgumentException<T>(T arg, string argName) where T : SafeHandle
    {
      if (argName == null)
        throw new ArgumentNullException(nameof (argName));
      if ((object) arg == null)
        throw new ArgumentNullException(argName);
      if (arg.IsInvalid)
        throw new ArgumentException(argName);
      return arg;
    }

    [DebuggerStepThrough]
    public static T CheckXmlNodeName<T>(T node, string expectedName) where T : XmlNode
    {
      Exceptions.CheckArgumentNullException<T>(node, nameof (node));
      Exceptions.CheckArgumentNullOrEmptyException(expectedName, nameof (expectedName));
      if (node.Name != expectedName)
        throw ExceptionFactory.XmlException("Incorrect xml node name: '{0}', expected: '{1}'", (object) node.Name, (object) expectedName);
      return node;
    }

    public static bool IsExistRecursive<T>(Exception ex) where T : Exception
    {
      return Exceptions.RecursiveTryExecute<bool>(ex, (Func<Exception, bool>) (exc => exc is T));
    }

    public static void RecursiveExecute(Exception ex, Action<Exception> func)
    {
      Exceptions.RecursiveTryExecute<object>(ex, (Func<Exception, object>) (exc =>
      {
        func(exc);
        return (object) null;
      }));
    }

    public static T RecursiveTryExecute<T>(Exception ex, Func<Exception, T> func)
    {
      if (ex == null)
        throw new ArgumentException(nameof (ex));
      Predicate<T> isDefaultResult = (Predicate<T>) (r => EqualityComparer<T>.Default.Equals(r, default (T)));
      T obj1 = func(ex);
      if (!isDefaultResult(obj1))
        return obj1;
      AggregateException aggregateException = ex as AggregateException;
      if (aggregateException != null)
        return aggregateException.InnerExceptions.Select<Exception, T>((Func<Exception, T>) (innerEx => Exceptions.RecursiveTryExecute<T>(innerEx, func))).Aggregate<T>((Func<T, T, T>) ((res1, res2) =>
        {
          if (!isDefaultResult(res1))
            return res1;
          return res2;
        }));
      if (ex.InnerException != null)
      {
        T obj2 = Exceptions.RecursiveTryExecute<T>(ex.InnerException, func);
        if (!isDefaultResult(obj2))
          return obj2;
      }
      return default (T);
    }

    public static IEnumerable<Exception> EnumerateExceptions(
      Exception ex,
      ExceptionTraversingOptions options)
    {
      while (ex != null)
      {
        AggregateException aggregate = ex as AggregateException;
        if (aggregate == null)
        {
          yield return ex;
          ex = ex.InnerException;
        }
        else
        {
          if ((options & ExceptionTraversingOptions.DontReturnAggregateException) != ExceptionTraversingOptions.DontReturnAggregateException)
            yield return ex;
          if ((options & ExceptionTraversingOptions.DontExpanAggregateException) != ExceptionTraversingOptions.DontExpanAggregateException)
          {
            foreach (Exception innerException in aggregate.InnerExceptions)
            {
              foreach (Exception enumerateException in Exceptions.EnumerateExceptions(innerException, options))
                yield return enumerateException;
            }
          }
          ex = (Exception) null;
        }
      }
    }
  }
}
