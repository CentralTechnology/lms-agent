using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Services.Authentication
{
    public class AuthenticationException : Exception
    {
        public AuthenticationException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
