using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class ManagedSupportUpdateModel
    {
        public DateTimeOffset CheckInTime { get; set; }
        public string ClientVersion { get; set; }
        public string Hostname { get; set; }
        public Portal.Common.Enums.CallInStatus Status { get; set; }
        public int UploadId { get; set; }
    }
}
