using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
  [ComVisible(true)]
  [Serializable]
  public class EnumeratorEx<TCollection, TEntry> : ICloneable, IEnumeratorEx<TEntry>, IEnumerator<TEntry>, IDisposable, IEnumerator
    where TCollection : class, IList<TEntry>
  {
    private TCollection _collection;
    private TEntry _currentElement;
    private int _index;

    public bool IsReverse { get; private set; }

    public int Index
    {
      get
      {
        return this._index;
      }
      set
      {
        this._index = value;
        if (this._index < 0 || this._index >= this._collection.Count)
          return;
        this._currentElement = this._collection[this._index];
      }
    }

    public int Step
    {
      get
      {
        return !this.IsReverse ? 1 : -1;
      }
    }

    public TEntry Current
    {
      get
      {
        if (this.Index < 0 || this.Index >= this._collection.Count)
          throw new IndexOutOfRangeException();
        return this._currentElement;
      }
    }

    public bool EndOfCollection
    {
      get
      {
        if (this.IsReverse)
          return this.Index < 0;
        return this.Index >= this._collection.Count;
      }
    }

    object IEnumerator.Current
    {
      get
      {
        return (object) this.Current;
      }
    }

    public EnumeratorEx(TCollection collection)
    {
      this._collection = collection;
      this.Index = -1;
      this.IsReverse = false;
    }

    [SecuritySafeCritical]
    public object Clone()
    {
      return this.MemberwiseClone();
    }

    public void Dispose()
    {
      if ((object) this._collection != null)
        this.Index = this._collection.Count;
      this.IsReverse = false;
      this._collection = default (TCollection);
    }

    public EnumeratorEx<TCollection, TEntry> Reverse(bool reset = true)
    {
      this.IsReverse = !this.IsReverse;
      if (reset)
        this.Reset();
      return this;
    }

    public bool MoveNext()
    {
      if (this.IsReverse)
      {
        --this.Index;
        if (this.Index >= 0)
          return true;
      }
      else
      {
        ++this.Index;
        if (this.Index < this._collection.Count)
          return true;
      }
      return false;
    }

    public bool MovePrevious()
    {
      if (this.IsReverse)
      {
        ++this.Index;
        if (this.Index < this._collection.Count)
          return true;
      }
      else
      {
        --this.Index;
        if (this.Index >= 0)
          return true;
      }
      return false;
    }

    public void Reset()
    {
      this.Index = this.IsReverse ? this._collection.Count : -1;
    }

    public void SetEnd()
    {
      this.Index = this.IsReverse ? 0 : this._collection.Count - 1;
    }
  }
}
