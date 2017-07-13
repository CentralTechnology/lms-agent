using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Factory
{
    using Users;

    public static class UserFactory
    {
        public static UserManager UserManager()
        {
            return new UserManager();
        }
    }
}
