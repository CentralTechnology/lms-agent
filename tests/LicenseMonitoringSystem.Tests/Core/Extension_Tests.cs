namespace LicenseMonitoringSystem.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using global::Core.Common.Extensions;
    using global::Core.Models;
    using Shouldly;
    using Xunit;

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Extension_Tests
    {
        [Fact]
        public void ApplyUploadId_ShouldSetUploadId()
        {
            // arrange
            var users = new DefaultUserList();
            int uploadId = 123456;

            // act
            ListExtensions.ApplyUploadId(users,uploadId);

            // assert
            users.ShouldAllBe(x => x.ManagedSupportId.Equals(uploadId));
        }

        [Fact]
        public void FilterCreate_ShouldReturnNoUsers()
        {
            // arrange
            var remoteUsers = new DefaultUserList();
            var localUsers = new ComparisonUserList();

            // act
            List<LicenseUser> usersToCreate = remoteUsers.FilterCreate<LicenseUser, Guid>(localUsers);

            // assert
            usersToCreate.ShouldBeEmpty();
        }

        [Fact]
        public void FilterCreate_ShouldReturnOnlyUsersToBeCreated()
        {
            // arrange
            var remoteUsers = new ComparisonUserList();
            var localUsers = new DefaultUserList();

            // act
            List<LicenseUser> usersToCreate = remoteUsers.FilterCreate<LicenseUser, Guid>(localUsers);

            // assert
            usersToCreate.Count.ShouldBe(3);
            usersToCreate.ShouldContain(DefaultUserList.User1);
            usersToCreate.ShouldContain(DefaultUserList.User2);
            usersToCreate.ShouldContain(DefaultUserList.User3);
        }

        [Fact]
        public void FilterDelete_ShouldReturnOnlyUsersToBeDelete()
        {
            // arrange
            var remoteUsers = new DefaultUserList();
            var localUsers = new ComparisonUserList();

            // act
            List<LicenseUser> usersToDelete = remoteUsers.FilterDelete<LicenseUser, Guid>(localUsers);

            // assert
            usersToDelete.Count.ShouldBe(3);
            usersToDelete.ShouldContain(DefaultUserList.User1);
            usersToDelete.ShouldContain(DefaultUserList.User2);
            usersToDelete.ShouldContain(DefaultUserList.User3);
        }

        [Fact]
        public void FilterUpdate_ShouldReturnOnlyUsersToBeUpdated()
        {
            // arrange
            var remoteUsers = new DefaultUserList();
            var localUsers = new ComparisonUserList();

            // act
            List<LicenseUser> usersToUpdate = remoteUsers.FilterUpdate<LicenseUser, Guid>(localUsers);

            // assert
            usersToUpdate.Count.ShouldBe(2);
            usersToUpdate.ShouldContain(DefaultUserList.User4);
            usersToUpdate.ShouldContain(DefaultUserList.User5);
        }
    }

    public class DefaultUserList : List<LicenseUser>
    {
        public static LicenseUser User1 = new LicenseUser
        {
            Id = Guid.Parse("e4779953-59ae-4507-9112-d734ce032a8f"),
            DisplayName = "Thomas Adams"
        };

        public static LicenseUser User2 = new LicenseUser
        {
            Id = Guid.Parse("3a167b14-21fb-4408-a968-ad0f0b5b9879"),
            DisplayName = "Andy Summerfield"
        };

        public static LicenseUser User3 = new LicenseUser
        {
            Id = Guid.Parse("e01db876-a8ab-4d78-9865-19cb542889b6"),
            DisplayName = "Adam Jacques"
        };

        public static LicenseUser User4 = new LicenseUser
        {
            Id = Guid.Parse("a15e0e50-529a-4028-8ba7-771019ca2fb7"),
            DisplayName = "Chris Barr"
        };

        public static LicenseUser User5 = new LicenseUser
        {
            Id = Guid.Parse("161dfdf5-e624-4d1f-8226-a0e456f92f16"),
            DisplayName = "Adam Collin"
        };

        public DefaultUserList()
        {
            AddRange(new List<LicenseUser>
            {
                User1,
                User2,
                User3,
                User4,
                User5
            });
        }
    }

    public class ComparisonUserList : List<LicenseUser>
    {
        public ComparisonUserList()
        {
            AddRange(new List<LicenseUser>
            {
                DefaultUserList.User4,
                DefaultUserList.User5
            });
        }
    }
}