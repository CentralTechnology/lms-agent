using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.Common
{
    public class CTapeDriveRegValue
    {
        private string _driveSysName;
        private int _driveNumInChanger;

        public CTapeDriveRegValue(string driveSysName, int driveNumInChanger)
        {
            this._driveSysName = driveSysName;
            this._driveNumInChanger = driveNumInChanger;
        }

        public string DriveSysName
        {
            get
            {
                return this._driveSysName;
            }
        }

        public int DriveNumInChanger
        {
            get
            {
                return this._driveNumInChanger;
            }
        }
    }
}
