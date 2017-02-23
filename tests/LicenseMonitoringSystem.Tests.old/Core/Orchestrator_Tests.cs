using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseMonitoringSystem.Tests.Core
{
    using LicenseMonitoringSystem.Core.Users;
    using NSubstitute;

    public class Orchestrator_Tests
    {
        private readonly IUserManager _userManager;

        public Orchestrator_Tests()
        {
            _userManager = Substitute.For<IUserManager>();
        }
    }
}
