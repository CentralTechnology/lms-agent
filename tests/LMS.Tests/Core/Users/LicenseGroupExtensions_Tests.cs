using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseMonitoringSystem.Tests.Core.Users
{
    using LMS.Users.Extensions;
    using Portal.LicenseMonitoringSystem.Users.Entities;
    using Shouldly;
    using Xunit;

    public  class LicenseGroupExtensions_Tests
    {
        [Fact]
        public void Format_ShouldReturnName_WhenNameIsPopulated()
        {
            // arrange
            var sut = new LicenseGroup
            {
                Name = "Test Group"
            };

            // act
            var result = sut.Format();

            // assert
            result.ShouldBe("Test Group");
        }

        [Fact]
        public void Format_ShouldReturnId_WhenNameIsNotPopulated()
        {
            // arrange
            var sut = new LicenseGroup
            {
                Id = Guid.Parse("8678fcae-916b-4f6e-a3e8-6b0ded8be4d1")
            };

            // act
            var result = sut.Format();

            // assert
            result.ShouldBe("8678fcae-916b-4f6e-a3e8-6b0ded8be4d1");
        }

        [Fact]
        public void Format_ShouldReturnNameAndId_WhenNameIsPopulated_WhenDebugIsTrue()
        {
            // arrange
            var sut = new LicenseGroup
            {
                Name = "Test Group",
                Id = Guid.Parse("8678fcae-916b-4f6e-a3e8-6b0ded8be4d1")
            };

            // act
            var result = sut.Format(true);

            // assert
            result.ShouldBe("Test Group - 8678fcae-916b-4f6e-a3e8-6b0ded8be4d1");
        }
    }
}
