using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Common
{
    public class UnsupportedVersionException : Exception
    {
        public UnsupportedVersionException(string message)
            : base(message)
        {

        }
    }
}
