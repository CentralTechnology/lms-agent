using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Serilog;

namespace LMS.Core.Veeam.Backup.Common
{
  public class CCppComponentExceptionBuilder
  {
    private readonly CCppTraceEventContainer _traceEvents = new CCppTraceEventContainer();
    private readonly List<Exception> _exceptions = new List<Exception>();
    private const string ERROR_TRACE_PREFIX = "--tr:";
    private const string ERROR_TRACE_EVENT_PREFIX = "--tr:event:";
    private const string ERROR_TRACE_PROPERTY_PREFIX = "--tr:property:";
    private readonly int _errorCode;
    private readonly string _errorMessage;
    private CCppComponentExceptionBuilder.CSubExceptionBuilder _currBuilder;
    private bool _createNextExceptionAsComException;

    private CCppComponentExceptionBuilder(
      int errorCode,
      string errorMessage,
      bool createNextExceptionAsComException)
    {
      this._createNextExceptionAsComException = createNextExceptionAsComException;
      this._errorCode = errorCode;
      this._errorMessage = errorMessage;
    }

    public static CCppComponentExceptionBuilder Create()
    {
      return new CCppComponentExceptionBuilder(0, string.Empty, false);
    }

    public static CCppComponentExceptionBuilder CreateForComException(
      int errorCode,
      string errorMessage)
    {
      return new CCppComponentExceptionBuilder(errorCode, errorMessage, true);
    }

    public void AddMessage(string line)
    {
      if (CCppComponentExceptionBuilder.IsUserMsg(line))
      {
        this.PushNextException();
        this._currBuilder = this.CreateNextSubExceptionBuilder(line);
      }
      else if (this._currBuilder != null)
      {
        if (CCppComponentExceptionBuilder.IsTrace(line))
          this._currBuilder.AddTrace(CCppComponentExceptionBuilder.FormatTrace(line));
        else if (CCppComponentExceptionBuilder.IsTraceEvent(line))
        {
          CTraceEventInfo traceEvent = CCppComponentExceptionBuilder.TryParseTraceEvent(line);
          if (traceEvent == null)
          {
            Log.Warning("Invalid trace event: {0}.", (object) line);
          }
          else
          {
            this._traceEvents.Add(traceEvent);
            this._currBuilder.AddTrace(CCppComponentExceptionBuilder.FormatTraveEvent(traceEvent));
          }
        }
        else
        {
          if (!CCppComponentExceptionBuilder.IsTraceProperty(line))
            return;
          CTraceProperty traceProperty = CCppComponentExceptionBuilder.TryParseTraceProperty(line);
          if (traceProperty == null)
          {
            Log.Warning("Invalid trace property: {0}.", (object) line);
          }
          else
          {
            this._traceEvents.Add(traceProperty);
            this._currBuilder.AddTrace(CCppComponentExceptionBuilder.FormatTraceProperty(traceProperty));
          }
        }
      }
      else
        Log.Warning("[CppComponentExceptionBuilder] Unexpected error message line: {0}", (object) line);
    }

    public Exception GenerateResult()
    {
      this.PushNextException();
      return (Exception) new AggregateException(!string.IsNullOrEmpty(this._errorMessage) ? this._errorMessage : this._exceptions.Aggregate<Exception, StringBuilder>(new StringBuilder(), (Func<StringBuilder, Exception, StringBuilder>) ((sb, ex) => sb.AppendLine(ex.Message))).ToString(), (IEnumerable<Exception>) this._exceptions);
    }

    private void PushNextException()
    {
      if (this._currBuilder == null)
        return;
      this._exceptions.Add(this._currBuilder.GetException(this._traceEvents));
      this._currBuilder = (CCppComponentExceptionBuilder.CSubExceptionBuilder) null;
    }

    private CCppComponentExceptionBuilder.CSubExceptionBuilder CreateNextSubExceptionBuilder(
      string line)
    {
      if (!this._createNextExceptionAsComException)
        return CCppComponentExceptionBuilder.CSubExceptionBuilder.Create(line);
      this._createNextExceptionAsComException = false;
      return CCppComponentExceptionBuilder.CSubExceptionBuilder.Create(line, this._errorCode);
    }

    public static string RemoveTrace(string msg)
    {
      return msg.Substring("--tr:".Length);
    }

    private static string FormatTrace(string traceMsg)
    {
      return string.Format("   in c++: {0}", (object) CCppComponentExceptionBuilder.RemoveTrace(traceMsg));
    }

    private static CTraceEventInfo TryParseTraceEvent(string traceEventMsg)
    {
      if (!CCppComponentExceptionBuilder.IsTraceEvent(traceEventMsg))
        return (CTraceEventInfo) null;
      string[] strArray = CCppComponentExceptionBuilder.RemoveTraceEventPrefix(traceEventMsg).Split(':');
      if (strArray.Length < 1)
        return (CTraceEventInfo) null;
      return new CTraceEventInfo((ETraceEvent) Enum.Parse(typeof (ETraceEvent), strArray[0]), strArray.Length >= 2 ? strArray[1] : string.Empty);
    }

    private static string RemoveTraceEventPrefix(string traceEventMsg)
    {
      return traceEventMsg.Substring("--tr:event:".Length);
    }

    private static string FormatTraveEvent(CTraceEventInfo traceEvt)
    {
      return string.Format("   in c++ event: {0}", (object) traceEvt.ToString());
    }

    private static CTraceProperty TryParseTraceProperty(string tracePropMsg)
    {
      if (!CCppComponentExceptionBuilder.IsTraceProperty(tracePropMsg))
        return (CTraceProperty) null;
      string[] strArray = CCppComponentExceptionBuilder.RemoveTracePropertyPrefix(tracePropMsg).Split(':');
      if (strArray.Length < 1)
        return (CTraceProperty) null;
      return new CTraceProperty(strArray[0], strArray.Length > 1 ? strArray[1] : string.Empty);
    }

    private static string RemoveTracePropertyPrefix(string tracePropMsg)
    {
      return tracePropMsg.Substring("--tr:property:".Length);
    }

    private static string FormatTraceProperty(CTraceProperty prop)
    {
      return string.Format("   in c++ property: {0}", (object) prop.ToString());
    }

    public static bool IsTrace(string msg)
    {
      if (msg.StartsWith("--tr:") && !CCppComponentExceptionBuilder.IsTraceEvent(msg))
        return !CCppComponentExceptionBuilder.IsTraceProperty(msg);
      return false;
    }

    public static bool IsTraceEvent(string msg)
    {
      return msg.StartsWith("--tr:event:");
    }

    public static bool IsTraceProperty(string msg)
    {
      return msg.StartsWith("--tr:property:");
    }

    public static bool IsUserMsg(string msg)
    {
      if (!CCppComponentExceptionBuilder.IsTrace(msg) && !CCppComponentExceptionBuilder.IsTraceEvent(msg))
        return !CCppComponentExceptionBuilder.IsTraceProperty(msg);
      return false;
    }

    private class CSubExceptionBuilder
    {
      private readonly StringBuilder _stackTraceAccum = new StringBuilder();
      private readonly string _userMsg;
      private readonly bool _createComException;
      private readonly int _errorCode;

      public CSubExceptionBuilder(string userMsg, bool createComException, int errorCode)
      {
        this._userMsg = userMsg;
        this._createComException = createComException;
        this._errorCode = errorCode;
      }

      public static CCppComponentExceptionBuilder.CSubExceptionBuilder Create(
        string userMsg)
      {
        return new CCppComponentExceptionBuilder.CSubExceptionBuilder(userMsg, false, 0);
      }

      public static CCppComponentExceptionBuilder.CSubExceptionBuilder Create(
        string userMsg,
        int errorCode)
      {
        return new CCppComponentExceptionBuilder.CSubExceptionBuilder(userMsg, true, errorCode);
      }

      public Exception GetException(CCppTraceEventContainer traceEventsContainer)
      {
        if (!this._createComException)
          return (Exception) new CCppComponentException(this._userMsg, this._stackTraceAccum.ToString(), traceEventsContainer);
        return (Exception) new CCppCOMException(this._userMsg, this._stackTraceAccum.ToString(), traceEventsContainer, this._errorCode);
      }

      public void AddTrace(string str)
      {
        this._stackTraceAccum.AppendLine(str);
      }
    }
  }
}
