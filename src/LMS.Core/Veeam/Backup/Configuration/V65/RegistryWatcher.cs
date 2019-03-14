using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

namespace LMS.Core.Veeam.Backup.Configuration.V65
{
  public class RegistryWatcher : IDisposable
  {
    private readonly RegistryKey _key;
    private readonly SafeRegistryHandle _keyHandle;
    private readonly bool _subtree;
    private readonly RegistryWatcher.Filter _filter;
    private readonly ManualResetEvent _stopEvent;
    private Thread _watchingThread;
    private bool _disposed;
    private Exception _unexpectedError;

    public event Action<string> Removed;

    public event Action<RegistryKey> Changed;

    public event Action<Exception> Terminated;

    public RegistryWatcher(
      RegistryKey registryKey,
      bool subtree,
      bool keys,
      bool values,
      bool attributes,
      bool security)
    {
      if (registryKey == null)
        throw new ArgumentNullException(nameof (registryKey));
      if (!RegistryWatcher.HasNotifyPermission(registryKey))
        throw new ArgumentException(string.Format("Cannot create a RegistryWatcher for the key {0} because it hasn't the Notify access right.", (object) registryKey.Name), nameof (registryKey));
      this._key = registryKey.Reopen();
      this._keyHandle = this._key.Handle;
      this._subtree = subtree;
      this._filter = RegistryWatcher.PrepareFilter(keys, values, attributes, security);
      this._stopEvent = new ManualResetEvent(false);
    }

    public bool IsWatching
    {
      get
      {
        return this._watchingThread != null;
      }
    }

    public void Dispose()
    {
      if (!this.TryStopSafe())
        return;
      this.Terminate();
    }

    private void Terminate()
    {
      this._disposed = true;
      this._keyHandle.Dispose();
      this._stopEvent.Dispose();
    }

    public void Start()
    {
      this.CheckUnexpectedlyTerminated();
      this.CheckDisposed();
      lock (this._stopEvent)
      {
        if (this._watchingThread != null)
          return;
        this._watchingThread = new Thread(new ThreadStart(this.WatcherLoopSafe));
        this._watchingThread.Name = string.Format("RegistryWatcher: {0}", (object) this._key.Name);
        this._watchingThread.IsBackground = true;
        this._watchingThread.Start();
      }
    }

    private bool TryStopSafe()
    {
      try
      {
        if (this._unexpectedError != null || this._disposed)
          return false;
        lock (this._stopEvent)
        {
          Thread watchingThread = this._watchingThread;
          if (watchingThread == null)
            return true;
          this._stopEvent.Set();
          return watchingThread.Join(TimeSpan.FromMinutes(1.0));
        }
      }
      catch
      {
        return false;
      }
    }

    private static bool HasNotifyPermission(RegistryKey registryKey)
    {
      foreach (RegistryAccessRule accessRule in (ReadOnlyCollectionBase) registryKey.GetAccessControl(AccessControlSections.Access).GetAccessRules(true, true, typeof (SecurityIdentifier)))
      {
        if ((accessRule.RegistryRights & RegistryRights.Notify) == RegistryRights.Notify)
          return true;
      }
      return false;
    }

    private static RegistryWatcher.Filter PrepareFilter(
      bool keys,
      bool values,
      bool attributes,
      bool security)
    {
      RegistryWatcher.Filter filter = (RegistryWatcher.Filter) 0;
      if (keys)
        filter |= RegistryWatcher.Filter.Key;
      if (values)
        filter |= RegistryWatcher.Filter.Value;
      if (attributes)
        filter |= RegistryWatcher.Filter.Attributes;
      if (security)
        filter |= RegistryWatcher.Filter.Security;
      if (filter == (RegistryWatcher.Filter) 0)
        throw new ArgumentException("You must specify at least one monitored argument.");
      return filter;
    }

    private void WatcherLoopSafe()
    {
      try
      {
        AutoResetEvent autoResetEvent = new AutoResetEvent(false);
        WaitHandle[] waitHandles = new WaitHandle[2]
        {
          (WaitHandle) this._stopEvent,
          (WaitHandle) autoResetEvent
        };
        RegistryWatcher.Native.RegistryWaitForChangesAsync(this._keyHandle, this._subtree, this._filter, autoResetEvent.SafeWaitHandle);
        while (WaitHandle.WaitAny(waitHandles) != 0)
        {
          RegistryWatcher.Native.RegistryWaitForChangesAsync(this._keyHandle, this._subtree, this._filter, autoResetEvent.SafeWaitHandle);
          Task.Run((Action) (() =>
          {
            using (RegistryKey registryKey = this._key.Reopen())
            {
              if (registryKey == null)
                RegistryWatcher.BroadcastEventSafe<string>(this.Removed, this._key.Name);
              else
                RegistryWatcher.BroadcastEventSafe<RegistryKey>(this.Changed, registryKey);
            }
          }));
        }
        this._stopEvent.Reset();
      }
      catch (Exception ex)
      {
        this._unexpectedError = ex;
        this._stopEvent.Reset();
        this.Terminate();
        RegistryWatcher.BroadcastEventSafe<Exception>(this.Terminated, ex);
      }
      finally
      {
        this._watchingThread = (Thread) null;
      }
    }

    private void CheckUnexpectedlyTerminated()
    {
      if (this._unexpectedError != null)
        throw new ObjectDisposedException(string.Format("RegistryWatcher: {0} unexpectedly finished work.", (object) this._key.Name), this._unexpectedError);
    }

    private void CheckDisposed()
    {
      if (this._disposed)
        throw new ObjectDisposedException(string.Format("RegistryWatcher: {0} is already disposed and cannot be started again.", (object) this._key.Name));
    }

    private static void BroadcastEventSafe<T>(Action<T> action, T value)
    {
      if (action == null)
        return;
      try
      {
        foreach (Action<T> action1 in action.GetInvocationList().Cast<Action<T>>())
        {
          try
          {
            action1(value);
          }
          catch
          {
          }
        }
      }
      catch
      {
      }
    }

    private static class Native
    {
      public static void RegistryWaitForChangesAsync(
        SafeRegistryHandle key,
        bool watchSubtree,
        RegistryWatcher.Filter filter,
        SafeWaitHandle eventHandle)
      {
        int error = RegistryWatcher.Native.RegNotifyChangeKeyValue(key, watchSubtree, filter, eventHandle, true);
        if (error != 0)
          throw new Win32Exception(error);
      }

      [DllImport("advapi32.dll")]
      private static extern int RegNotifyChangeKeyValue(
        SafeRegistryHandle key,
        bool watchSubtree,
        RegistryWatcher.Filter filter,
        SafeWaitHandle eventHandle,
        bool async);
    }

    [System.Flags]
    private enum Filter
    {
      Key = 1,
      Attributes = 2,
      Value = 4,
      Security = 8,
      ThreadAgnostic = 268435456, // 0x10000000
    }
  }
}
