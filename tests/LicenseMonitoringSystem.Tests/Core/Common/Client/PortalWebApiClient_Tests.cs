using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseMonitoringSystem.Tests.Core.Common.Client
{
    using NSubstitute;
    using Shouldly;
    using Xunit;

    public class PortalWebApiClient_Tests : LicenseMonitoringSystemTestBase
    {
        [Fact]
        public void GetAntiForgeryToken_ShouldReturnString()
        {
            // act
            var result = PortalWebApiClient.GetAntiForgeryToken();

            // assert
            result.ShouldNotBeNullOrEmpty();
            result.Length.ShouldBeGreaterThan(0);
        }
    }
}
