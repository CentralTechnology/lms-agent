using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LMS.Core.Serialization.Xml
{
  internal sealed class ArrayStringSequenceXmlReader : VirtualXmlReader
  {
    private readonly XmlReader _original;
    private readonly StringBuilder _sb;
    private char[] _sequence;
    private string _value;
    private int _index;
    private bool _end;

    public ArrayStringSequenceXmlReader(XmlReader original)
    {
      this._original = original;
      this._sb = new StringBuilder();
    }

    public override void Close()
    {
      this._sequence = (char[]) null;
      this._sb.Clear();
    }

    public override bool MoveToNextAttribute()
    {
      if (this._sequence == null)
      {
        int content = (int) this._original.MoveToContent();
        this._sequence = this._original.ReadElementContentAsString().ToCharArray();
      }
      bool flag = false;
      for (int index = this._index; index < this._sequence.Length; ++index)
      {
        char ch = this._sequence[index];
        switch (ch)
        {
          case ',':
            if (flag)
            {
              this._sb.Append(ch);
              flag = false;
              break;
            }
            this._value = this._sb.ToString();
            this._index = index + 2;
            this._sb.Length = 0;
            return true;
          case '~':
            flag = !flag;
            if (!flag)
            {
              this._sb.Append(ch);
              break;
            }
            break;
          default:
            this._sb.Append(ch);
            break;
        }
      }
      if (this._sb.Length <= 0 && this._end)
        return false;
      this._value = this._sb.ToString();
      this._index = this._sequence.Length;
      this._sb.Length = 0;
      this._end = true;
      return true;
    }

    public override XmlNodeType MoveToContent()
    {
      return this._original.NodeType;
    }

    public override XmlNodeType NodeType
    {
      get
      {
        return XmlNodeType.Attribute;
      }
    }

    public override Type ValueType
    {
      get
      {
        return this._original.ValueType;
      }
    }

    public override bool HasValue
    {
      get
      {
        return this._original.HasValue;
      }
    }

    public override string Value
    {
      get
      {
        return this._value;
      }
    }
  }
}
