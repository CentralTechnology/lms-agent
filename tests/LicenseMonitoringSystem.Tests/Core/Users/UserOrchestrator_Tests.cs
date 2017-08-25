namespace LicenseMonitoringSystem.Tests.Core.Users
{
    using System;
    using System.Collections.Generic;
    using global::Core.Models;
    using global::Core.Users;
    using global::Core.Users.Compare;
    using global::Core.Users.Entities;
    using KellermanSoftware.CompareNetObjects;
    using Shouldly;
    using Xunit;

    public class UserOrchestrator_Tests : LicenseMonitoringSystemTestBase
    {
        public UserOrchestrator_Tests()
        {
            _userOrchestrator = new UserOrchestratorTest();
        }

        private readonly UserOrchestratorTest _userOrchestrator;

        [Fact]
        public void GetUsersOrGroupsToCreate_ShouldReturn2Groups()
        {
            // assign
            var localGroups = new List<LicenseGroup>
            {
                new LicenseGroup {Name = "Group1", Id = Guid.Parse("594d8c64-37a9-4fa7-957c-2452653e32da")},
                new LicenseGroup {Name = "Group2", Id = Guid.Parse("36250b2f-7c56-4d07-9f01-4d2320139dc5")},
                new LicenseGroup {Name = "Group3", Id = Guid.Parse("c832fb2d-bdf0-49fb-a299-1d5c1c6b5d75")},
                new LicenseGroup {Name = "Group4", Id = Guid.Parse("33a320cb-51a2-4ab1-9d7e-81e593b2d1b7")},
                new LicenseGroup {Name = "Group5", Id = Guid.Parse("3dcbcd03-c726-4f60-a0a1-8c1a10be36a7")}
            };

            var apiGroups = new List<LicenseGroup>
            {
                new LicenseGroup {Name = "Group1", Id = Guid.Parse("594d8c64-37a9-4fa7-957c-2452653e32da")},
                new LicenseGroup {Name = "Group2", Id = Guid.Parse("36250b2f-7c56-4d07-9f01-4d2320139dc5")},
                new LicenseGroup {Name = "Group3", Id = Guid.Parse("c832fb2d-bdf0-49fb-a299-1d5c1c6b5d75")}
            };

            // act
            List<LicenseGroup> groupsToCreate = _userOrchestrator.ExposeGetUsersOrGroupsToCreate(localGroups, apiGroups);

            // asset
            groupsToCreate.Count.ShouldBe(2);
        }

        [Fact]
        public void GetUsersOrGroupsToCreate_ShouldReturnLocalGroupList_WhenApiListIsNull()
        {
            // assign
            var localGroups = new List<LicenseGroup>
            {
                new LicenseGroup {Name = "Group1", Id = Guid.Parse("594d8c64-37a9-4fa7-957c-2452653e32da")},
                new LicenseGroup {Name = "Group2", Id = Guid.Parse("36250b2f-7c56-4d07-9f01-4d2320139dc5")},
                new LicenseGroup {Name = "Group3", Id = Guid.Parse("c832fb2d-bdf0-49fb-a299-1d5c1c6b5d75")},
                new LicenseGroup {Name = "Group4", Id = Guid.Parse("33a320cb-51a2-4ab1-9d7e-81e593b2d1b7")},
                new LicenseGroup {Name = "Group5", Id = Guid.Parse("3dcbcd03-c726-4f60-a0a1-8c1a10be36a7")}
            };

            // act
            List<LicenseGroup> groupsToCreate = _userOrchestrator.ExposeGetUsersOrGroupsToCreate(localGroups, null, 1234);

            // asset
            groupsToCreate.ShouldBeOfType(typeof(List<LicenseGroup>));
            groupsToCreate.Count.ShouldBe(5);
        }

        [Fact]
        public void GetUsersOrGroupsToCreate_ShouldReturnLocalUserList_WhenApiListIsNull()
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
            List<LicenseUser> usersToCreate = _userOrchestrator.ExposeGetUsersOrGroupsToCreate(localUsers, null, 1234);

            // asset
            usersToCreate.Count.ShouldBe(5);
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
            List<LicenseUser> usersToCreate = _userOrchestrator.ExposeGetUsersOrGroupsToCreate(localUsers, apiUsers, 1234);

            // asset
            usersToCreate.Count.ShouldBe(2);
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
            List<LicenseUser> usersToDelete = _userOrchestrator.ExposeGetUsersOrGroupsToDelete(localUsers, apiUsers);

            // asset
            usersToDelete.Count.ShouldBe(3);
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
            List<LicenseUser> usersToDelete = _userOrchestrator.ExposeGetUsersOrGroupsToDelete(localUsers, apiUsers);

            // asset
            usersToDelete.Count.ShouldBe(0);
        }

        [Fact]
        public void GetUsersToDelete_ShouldReturnEmptyList_WhenSourceValuesAreNull()
        {
            // act
            List<LicenseUser> usersToUpdate = _userOrchestrator.ExposeGetUsersOrGroupsToDelete(null, null);

            // asset
            usersToUpdate.Count.ShouldBe(0);
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
            List<LicenseUser> usersToUpdate = _userOrchestrator.ExposeGetUsersOrGroupsToUpdate<LicenseUser, LicenseUserCompareLogic>(localUsers, apiUsers);

            // asset
            usersToUpdate.Count.ShouldBe(5);
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
            List<LicenseUser> usersToUpdate = _userOrchestrator.ExposeGetUsersOrGroupsToUpdate<LicenseUser, LicenseUserCompareLogic>(localUsers, apiUsers);

            // asset
            usersToUpdate.Count.ShouldBe(0);
        }

        [Fact]
        public void GetUsersToUpdate_ShouldReturnEmptyList_WhenSourceValuesAreNull()
        {
            // act
            List<LicenseUser> usersToUpdate = _userOrchestrator.ExposeGetUsersOrGroupsToUpdate<LicenseUser, LicenseUserCompareLogic>(null, null);

            // asset
            usersToUpdate.Count.ShouldBe(0);
        }
    }

    public class UserOrchestratorTest : UserOrchestrator
    {
        public List<TEntity> ExposeGetUsersOrGroupsToCreate<TEntity>(List<TEntity> localEntities, List<TEntity> apiEntities, int uploadId = 0)
            where TEntity : LicenseBase
        {
            return GetUsersOrGroupsToCreate(localEntities, apiEntities, uploadId);
        }

        public List<LicenseUser> ExposeGetUsersOrGroupsToDelete(List<LicenseUser> localUsers, List<LicenseUser> apiUsers)
        {
            return GetUsersOrGroupsToDelete(localUsers, apiUsers);
        }

        public List<TEntity> ExposeGetUsersOrGroupsToUpdate<TEntity, TCompareLogic>(List<TEntity> localUsers, List<TEntity> apiUsers)
            where TEntity : LicenseBase
            where TCompareLogic : CompareLogic, new()
        {
            return GetUsersOrGroupsToUpdate<TEntity, TCompareLogic>(localUsers, apiUsers);
        }
    }
}