using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Veeam.Backup.DBManager
{
  public interface IDBManager
  {
    CLicensingDbScope Licensing { get; }
    void Dispose();
  }
}
