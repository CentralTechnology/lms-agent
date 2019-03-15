using System;
using System.Xml;
using LMS.Core.Veeam.Backup.Core;

namespace LMS.Core.Veeam.Backup.Common
{
    [Serializable]
    public class CScalarRetValue
    {
        public CScalarRetValue(object value)
        {
            this.Value = value;
        }

        public object Value { get; private set; }

        public override string ToString()
        {
            return this.Value.ToString();
        }

        public void Serial(XmlNode node)
        {
            try
            {
                CXmlHelper2.AddAttr(node, "value", CProxyBinaryFormatter.Serialize(this.Value));
            }
            catch (Exception ex)
            {
                CExceptionUtil.RegenTraceExc(ex, "Failed to serial CScalarRetValue.");
                throw;
            }
        }

        public static CScalarRetValue Unserial(XmlNode node)
        {
            try
            {
                return new CScalarRetValue(CProxyBinaryFormatter.Deserialize<object>(CXmlHelper2.GetAttrAsString(node, "value")));
            }
            catch (Exception ex)
            {
                CExceptionUtil.RegenTraceExc(ex, "Failed to unserial CScalarRetValue. Xml: [{0}]", (object) node);
                throw;
            }
        }
    }
}
