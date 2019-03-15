using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
    public class CTapeChangerRegValue
    {
        private string _changerSysName;
        private List<CTapeDriveRegValue> _drives;

        public CTapeChangerRegValue(string changerSysName)
        {
            this._changerSysName = changerSysName;
            this._drives = new List<CTapeDriveRegValue>();
        }

        public string ChangerSysName
        {
            get
            {
                return this._changerSysName;
            }
        }

        public CTapeDriveRegValue[] Drives
        {
            get
            {
                return this._drives.ToArray();
            }
        }

        public void AddDrive(CTapeDriveRegValue drive)
        {
            this._drives.Add(drive);
        }

        public int DrivesCount
        {
            get
            {
                return this._drives.Count;
            }
        }
    }
}
