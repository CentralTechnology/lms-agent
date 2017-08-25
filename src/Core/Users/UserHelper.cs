using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Users
{
    using Models;

    public static class UserHelper
    {
        public static void SetUpload(this LicenseUser user, int uploadId) => user.ManagedSupportId = uploadId;
    }
}
