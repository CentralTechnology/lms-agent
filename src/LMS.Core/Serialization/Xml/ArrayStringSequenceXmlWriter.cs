using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LMS.Core.Serialization.Xml
{
    internal sealed class ArrayStringSequenceXmlWriter : VirtualXmlWriter
    {
        private readonly XmlWriter _original;
        private readonly StringBuilder _sb;

        public ArrayStringSequenceXmlWriter(XmlWriter original)
        {
            this._original = original;
            this._sb = new StringBuilder();
        }

        public override void WriteValue(string value)
        {
            this.WriteString(value);
        }

        public override void WriteString(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof (text));
            if (text == string.Empty)
            {
                this._sb.Append(',');
                this._sb.Append(' ');
            }
            else
            {
                foreach (char ch in text)
                {
                    switch (ch)
                    {
                        case ',':
                        case '~':
                            this._sb.Append('~');
                            this._sb.Append(ch);
                            break;
                        default:
                            this._sb.Append(ch);
                            break;
                    }
                }
                this._sb.Append(',');
                this._sb.Append(' ');
            }
        }

        public override void Flush()
        {
            if (this._sb.Length > 2)
                this._sb.Length -= 2;
            this._original.WriteString(this._sb.ToString());
            this._sb.Length = 0;
        }

        public override void Close()
        {
            this.Flush();
        }
    }
}
