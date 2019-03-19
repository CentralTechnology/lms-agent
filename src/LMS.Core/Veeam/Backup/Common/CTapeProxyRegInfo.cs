using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
    public class CTapeProxyRegInfo
    {
        private string _proxyName;
        private List<CTapeChangerRegValue> _changers;

        public CTapeProxyRegInfo(string proxyName)
        {
            this._proxyName = proxyName;
            this._changers = new List<CTapeChangerRegValue>();
        }

        public CTapeProxyRegInfo()
            : this("")
        {
        }

        public string ProxyName
        {
            get
            {
                if (this._proxyName == "")
                    this._proxyName = this.GetBackupServerProxyName();
                return this._proxyName;
            }
        }

        public CTapeChangerRegValue[] Changers
        {
            get
            {
                return this._changers.ToArray();
            }
        }

        public bool ContainsAnyChanger
        {
            get
            {
                return this._changers.Any<CTapeChangerRegValue>((Func<CTapeChangerRegValue, bool>) (x => x.DrivesCount > 0));
            }
        }

        public bool ContainsChanger(string changerSysName)
        {
            return this._changers.Any<CTapeChangerRegValue>((Func<CTapeChangerRegValue, bool>) (x => string.Equals(x.ChangerSysName, changerSysName, StringComparison.CurrentCultureIgnoreCase)));
        }

        private string GetBackupServerProxyName()
        {
            return CHostName.TryGetLocalName(false);
        }

        public void AddChanger(CTapeChangerRegValue changer)
        {
            this._changers.Add(changer);
        }
    }
}
