using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Core.Common;

namespace LMS.Core.Veeam.Backup.Common
{
  public static class Guard
  {
    private const string DefaultUserMessage = "Internal error, see logs for details";

    public static void ArgumentNotNull(object argumentValue, string argumentName)
    {
      if (argumentValue == null)
        throw new ArgumentNullException(argumentName);
    }

    public static void ThrowIfNull(object varValue, string fmt, params object[] parms)
    {
      if (varValue == null)
        throw new ApplicationException(string.Format(fmt, parms));
    }

    public static void ThrowIfNotNull(object varValue, string fmt, params object[] parms)
    {
      if (varValue != null)
        throw new ApplicationException(string.Format(fmt, parms));
    }

    public static void ThrowCoreExceptionWithDefaultUserMessage(
      string textLogMessageFmt,
      params object[] parms)
    {
      throw new CCoreException("Internal error, see logs for details", string.Format(textLogMessageFmt, parms));
    }

    public static void ThrowCoreException(
      string userFriendlyMessage,
      string textLogMessageFmt,
      params object[] parms)
    {
      string logFileMessage = string.Format(textLogMessageFmt, parms);
      throw new CCoreException(userFriendlyMessage, logFileMessage);
    }

    public static void ThrowCoreExceptionIfNull(
      object varValue,
      string userFriendlyMessage,
      string textLogMessageFmt,
      params object[] parms)
    {
      if (varValue == null)
      {
        string logFileMessage = string.Format(textLogMessageFmt, parms);
        throw new CCoreException(userFriendlyMessage, logFileMessage);
      }
    }

    public static void ThrowCoreExceptionIfNull(
      object varValue,
      string textLogMessageFmt,
      params object[] parms)
    {
      if (varValue == null)
      {
        string str = string.Format(textLogMessageFmt, parms);
        throw new CCoreException(str, str);
      }
    }

    public static void ThrowCoreExceptionIfNullWithDefaultUserMessage(
      object varValue,
      string textLogMessageFmt,
      params object[] parms)
    {
      if (varValue == null)
        throw new CCoreException("Internal error, see logs for details", string.Format(textLogMessageFmt, parms));
    }

    public static void ThrowCoreExceptionIfFalse(
      bool condition,
      string userFriendlyMessage,
      string textLogMessageFmt,
      params object[] parms)
    {
      if (!condition)
      {
        string logFileMessage = string.Format(textLogMessageFmt, parms);
        throw new CCoreException(userFriendlyMessage, logFileMessage);
      }
    }

    public static void ArgumentNotEmpty(Guid argumentValue, string argumentName)
    {
      if (argumentValue == Guid.Empty)
        throw new ArgumentException(string.Empty, argumentName);
    }

    public static void ArgumentNotZero(long argumentValue, string argumentName)
    {
      if (argumentValue == 0L)
        throw new ArgumentException(string.Empty, argumentName);
    }

    public static void ArgumentNotNullOrEmpty(string argumentValue, string argumentName)
    {
      if (argumentValue == null)
        throw new ArgumentNullException(argumentName);
      if (argumentValue.Length == 0)
        throw new ArgumentException(string.Empty, argumentName);
    }

    public static void ArgumentNotNullOrWhiteSpace(string argumentValue, string argumentName)
    {
      if (argumentValue == null)
        throw new ArgumentNullException(argumentName);
      if (string.IsNullOrWhiteSpace(argumentValue))
        throw new ArgumentException(string.Empty, argumentName);
    }

    public static void TypeIsAssignable(
      System.Type assignmentTargetType,
      System.Type assignmentValueType,
      string argumentName)
    {
      if (assignmentTargetType == (System.Type) null)
        throw new ArgumentNullException(nameof (assignmentTargetType));
      if (assignmentValueType == (System.Type) null)
        throw new ArgumentNullException(nameof (assignmentValueType));
      if (!assignmentTargetType.IsAssignableFrom(assignmentValueType))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, string.Empty, (object) assignmentTargetType, (object) assignmentValueType), argumentName);
    }

    public static void InstanceIsAssignable(
      System.Type assignmentTargetType,
      object assignmentInstance,
      string argumentName)
    {
      if (assignmentTargetType == (System.Type) null)
        throw new ArgumentNullException(nameof (assignmentTargetType));
      if (assignmentInstance == null)
        throw new ArgumentNullException(nameof (assignmentInstance));
      if (!assignmentTargetType.IsInstanceOfType(assignmentInstance))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, string.Empty, (object) assignmentTargetType, (object) Guard.GetTypeName(assignmentInstance)), argumentName);
    }

    private static string GetTypeName(object assignmentInstance)
    {
      try
      {
        return assignmentInstance.GetType().FullName;
      }
      catch (Exception ex)
      {
        return string.Empty;
      }
    }

    public static void AreEqualCheck(object obj1, object obj2)
    {
      string errMessage = string.Format("Expected input value '{0}' equals to input value '{1}'", obj1, obj2);
      Guard.AreEqualCheck(obj1, obj2, errMessage);
    }

    public static void AreEqualCheck(object obj1, object obj2, string errMessage)
    {
      if (obj1.GetHashCode() != obj2.GetHashCode())
        throw ExceptionFactory.Create(errMessage);
    }

    public static void AreEqualCheck(object obj1, object obj2, string fmt, params object[] parms)
    {
      if (obj1.GetHashCode() != obj2.GetHashCode())
        throw ExceptionFactory.Create(string.Format(fmt, parms));
    }

    public static void AreNotEqualCheck(object obj1, object obj2, string errMessage)
    {
      if (obj1.GetHashCode() == obj2.GetHashCode())
        throw ExceptionFactory.Create(errMessage);
    }

    public static void AreNotEqualCheck(
      object obj1,
      object obj2,
      string fmt,
      params object[] parms)
    {
      if (obj1.GetHashCode() == obj2.GetHashCode())
        throw ExceptionFactory.Create(string.Format(fmt, parms));
    }

    public static void ThrowIfNoElements<T>(
      IEnumerable<T> elements,
      string fmt,
      params object[] parms)
    {
      if (!elements.Any<T>())
        throw ExceptionFactory.Create(string.Format(fmt, parms));
    }

    public static void ThrowIfNot(bool condition, string fmt, params object[] parms)
    {
      if (!condition)
        throw ExceptionFactory.Create(fmt, parms);
    }

    public static void Throw(string fmt, params object[] parms)
    {
      throw new ApplicationException(string.Format(fmt, parms));
    }

    public static void ThrowIf(bool condition, string fmt, params object[] parms)
    {
      if (condition)
        throw ExceptionFactory.Create(fmt, parms);
    }
  }
}
