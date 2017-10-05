namespace LicenseMonitoringSystem.Tests.Core.Users
{
    using System;
    using System.Collections.Generic;
    using global::Core.Users.Compare;
    using KellermanSoftware.CompareNetObjects;
    using Microsoft.OData.Client;
    using Portal.LicenseMonitoringSystem.Users.Entities;
    using Shouldly;
    using Xunit;

    public class LicenseUser_Tests : LicenseMonitoringSystemTestBase
    {
        [Fact]
        public void Compare_ShouldReturnFalse()
        {
            var compareLogic = new LicenseUserCompareLogic();

            var user1 = new LicenseUser
            {
                DisplayName = "John Doe",
                Email = "john.doe@example.com",
                Enabled = true,
                FirstName = "John",
                LastLoginDate = new DateTime(2017, 8, 25),
                Id = Guid.Parse("d03939b9-1f70-4c76-8cf0-ee121809591d"),
                SamAccountName = "john.doe",
                Surname = "Doe",
                WhenCreated = new DateTime(2012, 06, 4)
            };

            var user2 = new LicenseUser
            {
                DisplayName = "John Smith",
                Email = "john.smith@example.com",
                Enabled = true,
                FirstName = "John",
                LastLoginDate = new DateTime(2017, 8, 25),
                Id = Guid.Parse("b9702f8f-6617-45a6-9e75-41554d3a315c"),
                SamAccountName = "john.smith",
                Surname = "Smith",
                WhenCreated = new DateTime(2012, 06, 4)
            };

            ComparisonResult result = compareLogic.Compare(user1, user2);

            result.AreEqual.ShouldBe(false);
        }

        [Fact]
        public void Compare_ShouldReturnTrue()
        {
            var compareLogic = new LicenseUserCompareLogic();

            var user1 = new LicenseUser
            {
                DisplayName = "John Doe",
                Email = "john.doe@example.com",
                Enabled = true,
                FirstName = "John",
                LastLoginDate = new DateTime(2017, 8, 25),
                Id = Guid.Parse("d03939b9-1f70-4c76-8cf0-ee121809591d"),
                SamAccountName = "john.doe",
                Surname = "Doe",
                WhenCreated = new DateTime(2012, 06, 4)
            };

            var user2 = new LicenseUser
            {
                DisplayName = "John Doe",
                Email = "john.doe@example.com",
                Enabled = true,
                FirstName = "John",
                LastLoginDate = new DateTime(2017, 8, 25),
                Id = Guid.Parse("d03939b9-1f70-4c76-8cf0-ee121809591d"),
                SamAccountName = "john.doe",
                Surname = "Doe",
                WhenCreated = new DateTime(2012, 06, 4)
            };

            ComparisonResult result = compareLogic.Compare(user1, user2);

            result.AreEqual.ShouldBe(true);
        }

        [Fact]
        public void Compare_ShouldReturnTrue_WithExcludedProperties()
        {
            var compareLogic = new LicenseUserCompareLogic();

            var user1 = new LicenseUser
            {
                DisplayName = "John Doe",
                Email = "john.doe@example.com",
                Enabled = true,
                FirstName = "John",
                LastLoginDate = new DateTime(2017, 8, 25),
                Id = Guid.Parse("d03939b9-1f70-4c76-8cf0-ee121809591d"),
                SamAccountName = "john.doe",
                Surname = "Doe",
                WhenCreated = new DateTime(2012, 06, 4)
            };

            var user2 = new LicenseUser
            {
                DisplayName = "John Doe",
                Email = "john.doe@example.com",
                Enabled = true,
                FirstName = "John",
                LastLoginDate = new DateTime(2017, 8, 25),
                Groups = new DataServiceCollection<LicenseGroup>(new List<LicenseGroup>(), TrackingMode.None),
                Id = Guid.Parse("d03939b9-1f70-4c76-8cf0-ee121809591d"),
                ManagedSupportId = 5787878,
                SamAccountName = "john.doe",
                Surname = "Doe",
                WhenCreated = new DateTime(2012, 06, 4)
            };

            ComparisonResult result = compareLogic.Compare(user1, user2);

            result.AreEqual.ShouldBe(true);
        }
    }
}