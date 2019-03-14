using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
    [Serializable]
    public class CCoreException : CAppException
    {
        public string UserFriendlyMessage { get; private set; }

        public CCoreException(string userFriendlyMessage, string logFileMessage)
            : base(logFileMessage)
        {
            this.UserFriendlyMessage = userFriendlyMessage;
        }

        public CCoreException(
            string userFriendlyMessage,
            string logFileMessage,
            Exception innerException)
            : base(innerException, logFileMessage)
        {
            this.UserFriendlyMessage = userFriendlyMessage;
        }

        public void SetUserMessage(string fmt, params object[] args)
        {
            this.UserFriendlyMessage = string.Format(fmt, args);
        }
    }
}
