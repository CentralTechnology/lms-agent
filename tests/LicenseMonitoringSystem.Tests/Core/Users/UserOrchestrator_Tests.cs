namespace LicenseMonitoringSystem.Tests.Core.Users
{
    using System;
    using System.Collections.Generic;
    using global::Core.Models;
    using global::Core.Users;
    using Shouldly;
    using Xunit;

    public class UserOrchestrator_Tests : LicenseMonitoringSystemTestBase
    {
        private readonly UserOrchestratorTest _userOrchestrator;

        public UserOrchestrator_Tests()
        {
            _userOrchestrator = new UserOrchestratorTest();
        }

        [Fact]
        public void GetUsersToUpdate_ShouldReturn5Users()
        {
            // assign
            var localUsers = new List<LicenseUser>
            {
                new LicenseUser {DisplayName = "John Doe", Id = Guid.Parse("594d8c64-37a9-4fa7-957c-2452653e32da")},
                new LicenseUser {DisplayName = "Joe Scott", Id = Guid.Parse("36250b2f-7c56-4d07-9f01-4d2320139dc5")},
                new LicenseUser {DisplayName = "Hello Fresh", Id = Guid.Parse("c832fb2d-bdf0-49fb-a299-1d5c1c6b5d75")},
                new LicenseUser {DisplayName = "Jessica Remote", Id = Guid.Parse("33a320cb-51a2-4ab1-9d7e-81e593b2d1b7")},
                new LicenseUser {DisplayName = "September Phone", Id = Guid.Parse("3dcbcd03-c726-4f60-a0a1-8c1a10be36a7")}
            };

            var apiUsers = new List<LicenseUser>
            {
                new LicenseUser {DisplayName = "John Doe2", Id = Guid.Parse("594d8c64-37a9-4fa7-957c-2452653e32da")},
                new LicenseUser {DisplayName = "Joe Scott2", Id = Guid.Parse("36250b2f-7c56-4d07-9f01-4d2320139dc5")},
                new LicenseUser {DisplayName = "Hello Fresh2", Id = Guid.Parse("c832fb2d-bdf0-49fb-a299-1d5c1c6b5d75")},
                new LicenseUser {DisplayName = "Jessica Remote2", Id = Guid.Parse("33a320cb-51a2-4ab1-9d7e-81e593b2d1b7")},
                new LicenseUser {DisplayName = "September Phone2", Id = Guid.Parse("3dcbcd03-c726-4f60-a0a1-8c1a10be36a7")}
            };

            // act
            List<LicenseUser> usersToUpdate = _userOrchestrator.ExposeGetUsersToUpdate(localUsers, apiUsers);

            // asset
            usersToUpdate.Count.ShouldBe(5);
        }

        [Fact]
        public void GetUsersToUpdate_ShouldReturnEmptyList_WhenSourceValuesAreNull()
        {
            // act
            List<LicenseUser> usersToUpdate = _userOrchestrator.ExposeGetUsersToUpdate(null, null);

            // asset
            usersToUpdate.Count.ShouldBe(0);
        }

        [Fact]
        public void GetUsersToDelete_ShouldReturnEmptyList_WhenSourceValuesAreNull()
        {
            // act
            List<LicenseUser> usersToUpdate = _userOrchestrator.ExposeGetUsersToDelete(null, null);

            // asset
            usersToUpdate.Count.ShouldBe(0);
        }

        [Fact]
        public void GetUsersToUpdate_ShouldReturnEmptyList()
        {
            // assign
            var localUsers = new List<LicenseUser>
            {
                new LicenseUser {DisplayName = "John Doe", Id = Guid.Parse("594d8c64-37a9-4fa7-957c-2452653e32da")},
                new LicenseUser {DisplayName = "Joe Scott", Id = Guid.Parse("36250b2f-7c56-4d07-9f01-4d2320139dc5")},
                new LicenseUser {DisplayName = "Hello Fresh", Id = Guid.Parse("c832fb2d-bdf0-49fb-a299-1d5c1c6b5d75")},
                new LicenseUser {DisplayName = "Jessica Remote", Id = Guid.Parse("33a320cb-51a2-4ab1-9d7e-81e593b2d1b7")},
                new LicenseUser {DisplayName = "September Phone", Id = Guid.Parse("3dcbcd03-c726-4f60-a0a1-8c1a10be36a7")}
            };

            var apiUsers = new List<LicenseUser>
            {
                new LicenseUser {DisplayName = "John Doe", Id = Guid.Parse("594d8c64-37a9-4fa7-957c-2452653e32da")},
                new LicenseUser {DisplayName = "Joe Scott", Id = Guid.Parse("36250b2f-7c56-4d07-9f01-4d2320139dc5")},
                new LicenseUser {DisplayName = "Hello Fresh", Id = Guid.Parse("c832fb2d-bdf0-49fb-a299-1d5c1c6b5d75")},
                new LicenseUser {DisplayName = "Jessica Remote", Id = Guid.Parse("33a320cb-51a2-4ab1-9d7e-81e593b2d1b7")},
                new LicenseUser {DisplayName = "September Phone", Id = Guid.Parse("3dcbcd03-c726-4f60-a0a1-8c1a10be36a7")}
            };

            // act
            List<LicenseUser> usersToUpdate = _userOrchestrator.ExposeGetUsersToUpdate(localUsers, apiUsers);

            // asset
            usersToUpdate.Count.ShouldBe(0);
        }

        [Fact]
        public void GetUsersToDelete_ShouldReturnEmptyList()
        {
            // assign
            var localUsers = new List<LicenseUser>
            {
                new LicenseUser {DisplayName = "John Doe", Id = Guid.Parse("594d8c64-37a9-4fa7-957c-2452653e32da")},
                new LicenseUser {DisplayName = "Joe Scott", Id = Guid.Parse("36250b2f-7c56-4d07-9f01-4d2320139dc5")},
                new LicenseUser {DisplayName = "Hello Fresh", Id = Guid.Parse("c832fb2d-bdf0-49fb-a299-1d5c1c6b5d75")},
                new LicenseUser {DisplayName = "Jessica Remote", Id = Guid.Parse("33a320cb-51a2-4ab1-9d7e-81e593b2d1b7")},
                new LicenseUser {DisplayName = "September Phone", Id = Guid.Parse("3dcbcd03-c726-4f60-a0a1-8c1a10be36a7")}
            };

            var apiUsers = new List<LicenseUser>
            {
                new LicenseUser {DisplayName = "John Doe", Id = Guid.Parse("594d8c64-37a9-4fa7-957c-2452653e32da")},
                new LicenseUser {DisplayName = "Joe Scott", Id = Guid.Parse("36250b2f-7c56-4d07-9f01-4d2320139dc5")},
                new LicenseUser {DisplayName = "Hello Fresh", Id = Guid.Parse("c832fb2d-bdf0-49fb-a299-1d5c1c6b5d75")},
                new LicenseUser {DisplayName = "Jessica Remote", Id = Guid.Parse("33a320cb-51a2-4ab1-9d7e-81e593b2d1b7")},
                new LicenseUser {DisplayName = "September Phone", Id = Guid.Parse("3dcbcd03-c726-4f60-a0a1-8c1a10be36a7")}
            };

            // act
            List<LicenseUser> usersToDelete = _userOrchestrator.ExposeGetUsersToDelete(localUsers, apiUsers);

            // asset
            usersToDelete.Count.ShouldBe(0);
        }

        [Fact]
        public void GetUsersToDelete_ShouldReturn3Users()
        {
            // assign
            var localUsers = new List<LicenseUser>
            {
                new LicenseUser {DisplayName = "John Doe", Id = Guid.Parse("594d8c64-37a9-4fa7-957c-2452653e32da")},
                new LicenseUser {DisplayName = "Joe Scott", Id = Guid.Parse("36250b2f-7c56-4d07-9f01-4d2320139dc5")}
            };

            var apiUsers = new List<LicenseUser>
            {
                new LicenseUser {DisplayName = "John Doe", Id = Guid.Parse("594d8c64-37a9-4fa7-957c-2452653e32da")},
                new LicenseUser {DisplayName = "Joe Scott", Id = Guid.Parse("36250b2f-7c56-4d07-9f01-4d2320139dc5")},
                new LicenseUser {DisplayName = "Hello Fresh", Id = Guid.Parse("c832fb2d-bdf0-49fb-a299-1d5c1c6b5d75")},
                new LicenseUser {DisplayName = "Jessica Remote", Id = Guid.Parse("33a320cb-51a2-4ab1-9d7e-81e593b2d1b7")},
                new LicenseUser {DisplayName = "September Phone", Id = Guid.Parse("3dcbcd03-c726-4f60-a0a1-8c1a10be36a7")}
            };

            // act
            List<LicenseUser> usersToDelete = _userOrchestrator.ExposeGetUsersToDelete(localUsers, apiUsers);

            // asset
            usersToDelete.Count.ShouldBe(3);
        }

        [Fact]
        public void GetUsersToCreate_ShouldReturnLocalUserList_WhenApiListIsNull()
        {
            // assign
            var localUsers = new List<LicenseUser>
            {
                new LicenseUser {DisplayName = "John Doe", Id = Guid.Parse("594d8c64-37a9-4fa7-957c-2452653e32da")},
                new LicenseUser {DisplayName = "Joe Scott", Id = Guid.Parse("36250b2f-7c56-4d07-9f01-4d2320139dc5")},
                new LicenseUser {DisplayName = "Hello Fresh", Id = Guid.Parse("c832fb2d-bdf0-49fb-a299-1d5c1c6b5d75")},
                new LicenseUser {DisplayName = "Jessica Remote", Id = Guid.Parse("33a320cb-51a2-4ab1-9d7e-81e593b2d1b7")},
                new LicenseUser {DisplayName = "September Phone", Id = Guid.Parse("3dcbcd03-c726-4f60-a0a1-8c1a10be36a7")}
            };

            // act
            List<LicenseUser> usersToCreate = _userOrchestrator.ExposeGetUsersToCreate(localUsers, null, 1234);

            // asset
            usersToCreate.Count.ShouldBe(5);
            usersToCreate.ShouldBeSameAs(localUsers);
        }

        [Fact]
        public void GetUsersToCreate_ShouldReturn2Users()
        {
            // assign
            var localUsers = new List<LicenseUser>
            {
                new LicenseUser {DisplayName = "John Doe", Id = Guid.Parse("594d8c64-37a9-4fa7-957c-2452653e32da")},
                new LicenseUser {DisplayName = "Joe Scott", Id = Guid.Parse("36250b2f-7c56-4d07-9f01-4d2320139dc5")},
                new LicenseUser {DisplayName = "Hello Fresh", Id = Guid.Parse("c832fb2d-bdf0-49fb-a299-1d5c1c6b5d75")},
                new LicenseUser {DisplayName = "Jessica Remote", Id = Guid.Parse("33a320cb-51a2-4ab1-9d7e-81e593b2d1b7")},
                new LicenseUser {DisplayName = "September Phone", Id = Guid.Parse("3dcbcd03-c726-4f60-a0a1-8c1a10be36a7")}
            };

            var apiUsers = new List<LicenseUser>
            {
                new LicenseUser {DisplayName = "John Doe", Id = Guid.Parse("594d8c64-37a9-4fa7-957c-2452653e32da")},
                new LicenseUser {DisplayName = "Joe Scott", Id = Guid.Parse("36250b2f-7c56-4d07-9f01-4d2320139dc5")},
                new LicenseUser {DisplayName = "Hello Fresh", Id = Guid.Parse("c832fb2d-bdf0-49fb-a299-1d5c1c6b5d75")}
            };

            // act
            List<LicenseUser> usersToCreate = _userOrchestrator.ExposeGetUsersToCreate(localUsers, apiUsers, 1234);

            // asset
            usersToCreate.Count.ShouldBe(2);
        }
    }

    public class UserOrchestratorTest : UserOrchestrator
    {
        public List<LicenseUser> ExposeGetUsersToUpdate(List<LicenseUser> localUsers, List<LicenseUser> apiUsers)
        {
            return GetUsersToUpdate(localUsers, apiUsers);
        }

        public List<LicenseUser> ExposeGetUsersToDelete(List<LicenseUser> localUsers, List<LicenseUser> apiUsers)
        {
            return GetUsersToDelete(localUsers, apiUsers);
        }

        public List<LicenseUser> ExposeGetUsersToCreate(List<LicenseUser> localUsers, List<LicenseUser> apiUsers, int uploadId)
        {
            return GetUsersToCreate(localUsers, apiUsers, uploadId);
        }
    }
}