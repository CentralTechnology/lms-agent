using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Xml;

namespace LMS.Core.Veeam.Backup.Common
{
    [Serializable]
    public struct CTransactionScopeIdentifier
    {
        public Guid SessionId { get; set; }

        public int ProcessId { get; set; }

        public int ThreadId { get; set; }

        public static CTransactionScopeIdentifier CreateCurrent()
        {
            return CTransactionScopeIdentifier.CreateForSession(Guid.Empty);
        }

        public static CTransactionScopeIdentifier CreateForSession(
            Guid sessionId)
        {
            return new CTransactionScopeIdentifier()
            {
                SessionId = sessionId,
                ProcessId = Process.GetCurrentProcess().Id,
                ThreadId = Thread.CurrentThread.ManagedThreadId
            };
        }

        public string Serial()
        {
            XmlDocument doc = CXmlHelper2.CreateDoc(nameof (CTransactionScopeIdentifier));
            this.Serialize((XmlNode) doc.DocumentElement);
            return doc.DocumentElement.OuterXml;
        }

        public void Serialize(XmlNode node)
        {
            CXmlHelper2.AddAsInnerText(node, "SessionId", this.SessionId.ToString());
            CXmlHelper2.AddAsInnerText(node, "ProcessId", this.ProcessId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
            CXmlHelper2.AddAsInnerText(node, "ThreadId", this.ThreadId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        }

        public static CTransactionScopeIdentifier Unserial(XmlNode node)
        {
            return new CTransactionScopeIdentifier()
            {
                SessionId = CXmlHelper.GetChildNodeValue<Guid>(node, "SessionId", Guid.Empty),
                ProcessId = CXmlHelper.GetChildNodeValue<int>(node, "ProcessId", -1),
                ThreadId = CXmlHelper.GetChildNodeValue<int>(node, "ThreadId", -1)
            };
        }

        public static CTransactionScopeIdentifier Unserial(string transXml)
        {
            return CTransactionScopeIdentifier.Unserial((XmlNode) CXmlHelper2.LoadDoc(transXml, nameof (CTransactionScopeIdentifier)).DocumentElement);
        }
    }
}
