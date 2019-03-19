using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Core.Common;
using Serilog;

namespace LMS.Core.Veeam.Backup.Common
{
  public class CDisposableList : IDisposable, IEnumerable
  {
    private readonly Stack<IDisposable> _innerStack;
    private readonly bool _allowNullAsObjects;
    private readonly bool _throwItemDisposeException;

    public object SyncRoot
    {
      get
      {
        return ((ICollection) this._innerStack).SyncRoot;
      }
    }

    public CDisposableList(int capacity, bool throwItemDisposeException = false)
    {
      this._innerStack = new Stack<IDisposable>(capacity);
      this._throwItemDisposeException = throwItemDisposeException;
    }

    public CDisposableList(bool allowNullAsObjects)
      : this(4, false)
    {
      this._allowNullAsObjects = allowNullAsObjects;
    }

    public CDisposableList()
      : this(false)
    {
    }

    public CDisposableList(IEnumerable<IDisposable> objects)
      : this()
    {
      this.AddRange(objects);
    }

    public CDisposableList(IEnumerable objects)
      : this()
    {
      foreach (IDisposable disposableObject in objects)
        this.Add<IDisposable>(disposableObject);
    }

    public int Count
    {
      get
      {
        return this._innerStack.Count;
      }
    }

    public T Add<T>(T disposableObject) where T : IDisposable
    {
      if ((object) disposableObject == null && !this._allowNullAsObjects)
        throw new ArgumentNullException(nameof (disposableObject));
      this._innerStack.Push((IDisposable) disposableObject);
      return disposableObject;
    }

    public void AddRange(IEnumerable<IDisposable> disposableObjects)
    {
      if (disposableObjects == null)
        throw new ArgumentNullException(nameof (disposableObjects));
      foreach (IDisposable disposableObject in disposableObjects)
        this._innerStack.Push(disposableObject);
    }

    public void Dispose()
    {
      List<Exception> exceptionList = new List<Exception>();
      while (this._innerStack.Count > 0)
      {
        try
        {
          IDisposable disposable = this._innerStack.Pop();
          if (disposable != null)
          {
            Task task = disposable as Task;
            if (task != null && !task.IsCompleted)
              exceptionList.Add((Exception) new InvalidOperationException(string.Format("Task {0} is {1}. A task may only be disposed if it is in a completion state (RanToCompletion, Faulted or Canceled).", (object) task.Id, (object) task.Status)));
            else
              disposable.Dispose();
          }
        }
        catch (Exception ex)
        {
          if (this._throwItemDisposeException)
            throw;
          else
            exceptionList.Add(ex);
        }
      }
      try
      {
        if (exceptionList.Count > 0)
          throw ExceptionFactory.ThrowNecessaryAggregateException("Failed to release disposable resources.", (IEnumerable<Exception>) exceptionList);
      }
      catch (Exception ex)
      {
        if (this._throwItemDisposeException)
          throw;
        else
          Log.Error(ex, (string) null);
      }
    }

    public void Remove(IDisposable disposableObject)
    {
      if (disposableObject == null && !this._allowNullAsObjects)
        throw new ArgumentNullException(nameof (disposableObject));
      List<IDisposable> list = this._innerStack.ToList<IDisposable>();
      list.Reverse();
      if (!list.Remove(disposableObject))
        return;
      this._innerStack.Clear();
      this.AddRange((IEnumerable<IDisposable>) list);
    }

    public void SafeClear()
    {
      try
      {
        this._innerStack.Clear();
      }
      catch (Exception ex)
      {
        Log.Error(ex, (string) null);
      }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this._innerStack.GetEnumerator();
    }

    public IDisposable[] ToArray()
    {
      return this._innerStack.Reverse<IDisposable>().ToArray<IDisposable>();
    }
  }
}
