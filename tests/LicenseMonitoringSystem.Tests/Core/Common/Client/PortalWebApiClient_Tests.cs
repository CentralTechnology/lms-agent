using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseMonitoringSystem.Tests.Core.Common.Client
{
    using global::Core.Common.Client;
    using NSubstitute;
    using Shouldly;
    using Xunit;

    public class PortalWebApiClient_Tests : LicenseMonitoringSystemTestBase
    {
        [Fact]
        public async Task GetTokenCookie_ShouldReturnString()
        {
            // act
            var result = await PortalWebApiClient.GetTokenCookie();

            // assert
            result.ShouldNotBeNullOrEmpty();
            result.Length.ShouldBeGreaterThan(0);
        }
    }
}
