namespace LicenseMonitoringSystem.Tests.Core.Users
{
    using System;
    using System.Collections.Generic;
    using Abp.Timing;
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
            var localGroups = new List<LicenseGroup_Old>
            {
                new LicenseGroup_Old {Name = "Group1", Id = Guid.Parse("594d8c64-37a9-4fa7-957c-2452653e32da")},
                new LicenseGroup_Old {Name = "Group2", Id = Guid.Parse("36250b2f-7c56-4d07-9f01-4d2320139dc5")},
                new LicenseGroup_Old {Name = "Group3", Id = Guid.Parse("c832fb2d-bdf0-49fb-a299-1d5c1c6b5d75")},
                new LicenseGroup_Old {Name = "Group4", Id = Guid.Parse("33a320cb-51a2-4ab1-9d7e-81e593b2d1b7")},
                new LicenseGroup_Old {Name = "Group5", Id = Guid.Parse("3dcbcd03-c726-4f60-a0a1-8c1a10be36a7")}
            };

            var apiGroups = new List<LicenseGroup_Old>
            {
                new LicenseGroup_Old {Name = "Group1", Id = Guid.Parse("594d8c64-37a9-4fa7-957c-2452653e32da")},
                new LicenseGroup_Old {Name = "Group2", Id = Guid.Parse("36250b2f-7c56-4d07-9f01-4d2320139dc5")},
                new LicenseGroup_Old {Name = "Group3", Id = Guid.Parse("c832fb2d-bdf0-49fb-a299-1d5c1c6b5d75")}
            };

            // act
            List<LicenseGroup_Old> groupsToCreate = _userOrchestrator.ExposeGetUsersOrGroupsToCreate(localGroups, apiGroups);

            // asset
            groupsToCreate.Count.ShouldBe(2);
        }

        [Fact]
        public void GetUsersOrGroupsToCreate_ShouldReturnLocalGroupList_WhenApiListIsNull()
        {
            // assign
            var localGroups = new List<LicenseGroup_Old>
            {
                new LicenseGroup_Old {Name = "Group1", Id = Guid.Parse("594d8c64-37a9-4fa7-957c-2452653e32da")},
                new LicenseGroup_Old {Name = "Group2", Id = Guid.Parse("36250b2f-7c56-4d07-9f01-4d2320139dc5")},
                new LicenseGroup_Old {Name = "Group3", Id = Guid.Parse("c832fb2d-bdf0-49fb-a299-1d5c1c6b5d75")},
                new LicenseGroup_Old {Name = "Group4", Id = Guid.Parse("33a320cb-51a2-4ab1-9d7e-81e593b2d1b7")},
                new LicenseGroup_Old {Name = "Group5", Id = Guid.Parse("3dcbcd03-c726-4f60-a0a1-8c1a10be36a7")}
            };

            // act
            List<LicenseGroup_Old> groupsToCreate = _userOrchestrator.ExposeGetUsersOrGroupsToCreate(localGroups, null, 1234);

            // asset
            groupsToCreate.ShouldBeOfType(typeof(List<LicenseGroup_Old>));
            groupsToCreate.Count.ShouldBe(5);
        }

        [Fact]
        public void GetUsersOrGroupsToCreate_ShouldReturnLocalUserList_WhenApiListIsNull()
        {
            // assign
            var localUsers = new List<LicenseUser_Old>
            {
                new LicenseUser_Old {DisplayName = "John Doe", Id = Guid.Parse("594d8c64-37a9-4fa7-957c-2452653e32da")},
                new LicenseUser_Old {DisplayName = "Joe Scott", Id = Guid.Parse("36250b2f-7c56-4d07-9f01-4d2320139dc5")},
                new LicenseUser_Old {DisplayName = "Hello Fresh", Id = Guid.Parse("c832fb2d-bdf0-49fb-a299-1d5c1c6b5d75")},
                new LicenseUser_Old {DisplayName = "Jessica Remote", Id = Guid.Parse("33a320cb-51a2-4ab1-9d7e-81e593b2d1b7")},
                new LicenseUser_Old {DisplayName = "September Phone", Id = Guid.Parse("3dcbcd03-c726-4f60-a0a1-8c1a10be36a7")}
            };

            // act
            List<LicenseUser_Old> usersToCreate = _userOrchestrator.ExposeGetUsersOrGroupsToCreate(localUsers, null, 1234);

            // asset
            usersToCreate.Count.ShouldBe(5);
        }

        [Fact]
        public void GetUsersToCreate_ShouldReturn2Users()
        {
            // assign
            var localUsers = new List<LicenseUser_Old>
            {
                new LicenseUser_Old {DisplayName = "John Doe", Id = Guid.Parse("594d8c64-37a9-4fa7-957c-2452653e32da")},
                new LicenseUser_Old {DisplayName = "Joe Scott", Id = Guid.Parse("36250b2f-7c56-4d07-9f01-4d2320139dc5")},
                new LicenseUser_Old {DisplayName = "Hello Fresh", Id = Guid.Parse("c832fb2d-bdf0-49fb-a299-1d5c1c6b5d75")},
                new LicenseUser_Old {DisplayName = "Jessica Remote", Id = Guid.Parse("33a320cb-51a2-4ab1-9d7e-81e593b2d1b7")},
                new LicenseUser_Old {DisplayName = "September Phone", Id = Guid.Parse("3dcbcd03-c726-4f60-a0a1-8c1a10be36a7")}
            };

            var apiUsers = new List<LicenseUser_Old>
            {
                new LicenseUser_Old {DisplayName = "John Doe", Id = Guid.Parse("594d8c64-37a9-4fa7-957c-2452653e32da")},
                new LicenseUser_Old {DisplayName = "Joe Scott", Id = Guid.Parse("36250b2f-7c56-4d07-9f01-4d2320139dc5")},
                new LicenseUser_Old {DisplayName = "Hello Fresh", Id = Guid.Parse("c832fb2d-bdf0-49fb-a299-1d5c1c6b5d75")}
            };

            // act
            List<LicenseUser_Old> usersToCreate = _userOrchestrator.ExposeGetUsersOrGroupsToCreate(localUsers, apiUsers, 1234);

            // asset
            usersToCreate.Count.ShouldBe(2);
        }

        [Fact]
        public void GetUsersToDelete_ShouldReturn3Users()
        {
            // assign
            var localUsers = new List<LicenseUser_Old>
            {
                new LicenseUser_Old {DisplayName = "John Doe", Id = Guid.Parse("594d8c64-37a9-4fa7-957c-2452653e32da")},
                new LicenseUser_Old {DisplayName = "Joe Scott", Id = Guid.Parse("36250b2f-7c56-4d07-9f01-4d2320139dc5")}
            };

            var apiUsers = new List<LicenseUser_Old>
            {
                new LicenseUser_Old {DisplayName = "John Doe", Id = Guid.Parse("594d8c64-37a9-4fa7-957c-2452653e32da")},
                new LicenseUser_Old {DisplayName = "Joe Scott", Id = Guid.Parse("36250b2f-7c56-4d07-9f01-4d2320139dc5")},
                new LicenseUser_Old {DisplayName = "Hello Fresh", Id = Guid.Parse("c832fb2d-bdf0-49fb-a299-1d5c1c6b5d75")},
                new LicenseUser_Old {DisplayName = "Jessica Remote", Id = Guid.Parse("33a320cb-51a2-4ab1-9d7e-81e593b2d1b7")},
                new LicenseUser_Old {DisplayName = "September Phone", Id = Guid.Parse("3dcbcd03-c726-4f60-a0a1-8c1a10be36a7")}
            };

            // act
            List<LicenseUser_Old> usersToDelete = _userOrchestrator.ExposeGetUsersOrGroupsToDelete(localUsers, apiUsers);

            // asset
            usersToDelete.Count.ShouldBe(3);
        }

        [Fact]
        public void GetUsersToDelete_ShouldReturnEmptyList()
        {
            // assign
            var localUsers = new List<LicenseUser_Old>
            {
                new LicenseUser_Old {DisplayName = "John Doe", Id = Guid.Parse("594d8c64-37a9-4fa7-957c-2452653e32da")},
                new LicenseUser_Old {DisplayName = "Joe Scott", Id = Guid.Parse("36250b2f-7c56-4d07-9f01-4d2320139dc5")},
                new LicenseUser_Old {DisplayName = "Hello Fresh", Id = Guid.Parse("c832fb2d-bdf0-49fb-a299-1d5c1c6b5d75")},
                new LicenseUser_Old {DisplayName = "Jessica Remote", Id = Guid.Parse("33a320cb-51a2-4ab1-9d7e-81e593b2d1b7")},
                new LicenseUser_Old {DisplayName = "September Phone", Id = Guid.Parse("3dcbcd03-c726-4f60-a0a1-8c1a10be36a7")}
            };

            var apiUsers = new List<LicenseUser_Old>
            {
                new LicenseUser_Old {DisplayName = "John Doe", Id = Guid.Parse("594d8c64-37a9-4fa7-957c-2452653e32da")},
                new LicenseUser_Old {DisplayName = "Joe Scott", Id = Guid.Parse("36250b2f-7c56-4d07-9f01-4d2320139dc5")},
                new LicenseUser_Old {DisplayName = "Hello Fresh", Id = Guid.Parse("c832fb2d-bdf0-49fb-a299-1d5c1c6b5d75")},
                new LicenseUser_Old {DisplayName = "Jessica Remote", Id = Guid.Parse("33a320cb-51a2-4ab1-9d7e-81e593b2d1b7")},
                new LicenseUser_Old {DisplayName = "September Phone", Id = Guid.Parse("3dcbcd03-c726-4f60-a0a1-8c1a10be36a7")}
            };

            // act
            List<LicenseUser_Old> usersToDelete = _userOrchestrator.ExposeGetUsersOrGroupsToDelete(localUsers, apiUsers);

            // asset
            usersToDelete.Count.ShouldBe(0);
        }

        [Fact]
        public void GetUsersToDelete_ShouldReturnEmptyList_WhenSourceValuesAreNull()
        {
            // act
            List<LicenseUser_Old> usersToUpdate = _userOrchestrator.ExposeGetUsersOrGroupsToDelete(null, null);

            // asset
            usersToUpdate.Count.ShouldBe(0);
        }

        [Fact]
        public void GetUsersOrGroupsToUpdate_ShouldReturn3Users_AllProperties()
        {
            // assign
            var localUsers = new List<LicenseUser_Old>
            {
                new LicenseUser_Old
                {
                    DisplayName = "John Doe",
                    Email = "john.doe@example.com",
                    Enabled = true, FirstName = "John",
                    Groups = new List<LicenseGroup_Old>{ new LicenseGroup_Old{ Id = Guid.Parse("567b377c-6563-4f01-8ce1-dc7280ea228f"), Name = "Group1", WhenCreated = Clock.Now.AddDays(-1) }},
                    Id = Guid.Parse("594d8c64-37a9-4fa7-957c-2452653e32da"),
                    LastLoginDate = Clock.Now.AddHours(-1),
                    ManagedSupportId = 0,
                    SamAccountName = "john.doe",
                    Surname = "Doe",
                    WhenCreated = new DateTime(2015,12,2)
                },
                new LicenseUser_Old
                {
                    DisplayName = "Joe Scott",
                    Email = "joe.scott@example.co.uk",
                    Enabled = true,
                    FirstName = "Joe",
                    Groups = new List<LicenseGroup_Old>{ new LicenseGroup_Old { Id = Guid.Parse("f67964d0-7fe4-4af2-b95e-bdd5d21d6143"), Name = "Group1", WhenCreated = Clock.Now.AddMonths(-8)} },
                    Id = Guid.Parse("36250b2f-7c56-4d07-9f01-4d2320139dc5"),
                    LastLoginDate = new DateTime(2017,08,25,11,47,36),
                    SamAccountName = "joe.scott",
                    Surname = "Scott",
                    WhenCreated = new DateTime(2012,08,24)
                },
                new LicenseUser_Old
                {
                    DisplayName = "Walter Bishop",
                    Email = "walter.bishop@finge.co.uk",
                    Enabled = false,
                    FirstName = "Walter",
                    Id = Guid.Parse("c832fb2d-bdf0-49fb-a299-1d5c1c6b5d75"),
                    LastLoginDate = null,
                    SamAccountName = "walter.bishop",
Surname = "Bishop",
WhenCreated = Clock.Now
                },
                new LicenseUser_Old {DisplayName = "Jessica Remote", Id = Guid.Parse("33a320cb-51a2-4ab1-9d7e-81e593b2d1b7")},
                new LicenseUser_Old {DisplayName = "September Phone", Id = Guid.Parse("3dcbcd03-c726-4f60-a0a1-8c1a10be36a7")}
            };

            var apiUsers = new List<LicenseUser_Old>
            {
                // change of login date
                new LicenseUser_Old
                {
                    DisplayName = "John Doe",
                    Email = "john.doe@example.com",
                    Enabled = true, FirstName = "John",
                    Groups = new List<LicenseGroup_Old>{ new LicenseGroup_Old{ Id = Guid.Parse("567b377c-6563-4f01-8ce1-dc7280ea228f"), Name = "Group1", WhenCreated = Clock.Now.AddDays(-1) }},
                    Id = Guid.Parse("594d8c64-37a9-4fa7-957c-2452653e32da"),
                    LastLoginDate = Clock.Now.AddMinutes(-1),
                    ManagedSupportId = 0,
                    SamAccountName = "john.doe",
                    Surname = "Doe",
                    WhenCreated = new DateTime(2015,12,2)
                },

                // no change
                new LicenseUser_Old
                {
                    DisplayName = "Joe Scott",
                    Email = "joe.scott@example.co.uk",
                    Enabled = true,
                    FirstName = "Joe",
                    Groups = new List<LicenseGroup_Old>{ new LicenseGroup_Old { Id = Guid.Parse("f67964d0-7fe4-4af2-b95e-bdd5d21d6143"), Name = "Group1", WhenCreated = Clock.Now.AddMonths(-8)} },
                    Id = Guid.Parse("36250b2f-7c56-4d07-9f01-4d2320139dc5"),
                    LastLoginDate = new DateTime(2017,08,25,11,47,36),
                    SamAccountName = "joe.scott",
                    Surname = "Scott",
                    WhenCreated = new DateTime(2012,08,24)
                },

                // enabled = true
                new LicenseUser_Old
                {
                    DisplayName = "Walter Bishop",
                    Email = "walter.bishop@finge.co.uk",
                    Enabled = true,
                    FirstName = "Walter",
                    Id = Guid.Parse("c832fb2d-bdf0-49fb-a299-1d5c1c6b5d75"),
                    LastLoginDate = null,
                    SamAccountName = "walter.bishop",
                    Surname = "Bishop",
                    WhenCreated = Clock.Now
                },
                new LicenseUser_Old {DisplayName = "Jessica Remote2", Id = Guid.Parse("33a320cb-51a2-4ab1-9d7e-81e593b2d1b7")},
                new LicenseUser_Old {DisplayName = "September Phone", Id = Guid.Parse("3dcbcd03-c726-4f60-a0a1-8c1a10be36a7")}
            };

            // act
            List<LicenseUser_Old> usersToUpdate = _userOrchestrator.ExposeGetUsersOrGroupsToUpdate<LicenseUser_Old, LicenseUserCompareLogic>(localUsers, apiUsers);

            // asset
            usersToUpdate.Count.ShouldBe(3);
        }

        [Fact]
        public void GetUsersOrGroupsToUpdate_ShouldReturn5Users()
        {
            // assign
            var localUsers = new List<LicenseUser_Old>
            {
                new LicenseUser_Old {DisplayName = "John Doe", Id = Guid.Parse("594d8c64-37a9-4fa7-957c-2452653e32da")},
                new LicenseUser_Old {DisplayName = "Joe Scott", Id = Guid.Parse("36250b2f-7c56-4d07-9f01-4d2320139dc5")},
                new LicenseUser_Old {DisplayName = "Hello Fresh", Id = Guid.Parse("c832fb2d-bdf0-49fb-a299-1d5c1c6b5d75")},
                new LicenseUser_Old {DisplayName = "Jessica Remote", Id = Guid.Parse("33a320cb-51a2-4ab1-9d7e-81e593b2d1b7")},
                new LicenseUser_Old {DisplayName = "September Phone", Id = Guid.Parse("3dcbcd03-c726-4f60-a0a1-8c1a10be36a7")}
            };

            var apiUsers = new List<LicenseUser_Old>
            {
                new LicenseUser_Old {DisplayName = "John Doe2", Id = Guid.Parse("594d8c64-37a9-4fa7-957c-2452653e32da")},
                new LicenseUser_Old {DisplayName = "Joe Scott2", Id = Guid.Parse("36250b2f-7c56-4d07-9f01-4d2320139dc5")},
                new LicenseUser_Old {DisplayName = "Hello Fresh2", Id = Guid.Parse("c832fb2d-bdf0-49fb-a299-1d5c1c6b5d75")},
                new LicenseUser_Old {DisplayName = "Jessica Remote2", Id = Guid.Parse("33a320cb-51a2-4ab1-9d7e-81e593b2d1b7")},
                new LicenseUser_Old {DisplayName = "September Phone2", Id = Guid.Parse("3dcbcd03-c726-4f60-a0a1-8c1a10be36a7")}
            };

            // act
            List<LicenseUser_Old> usersToUpdate = _userOrchestrator.ExposeGetUsersOrGroupsToUpdate<LicenseUser_Old, LicenseUserCompareLogic>(localUsers, apiUsers);

            // asset
            usersToUpdate.Count.ShouldBe(5);
        }

        [Fact]
        public void GetUsersOrGroupsToUpdate_ShouldReturnEmptyList()
        {
            // assign
            var localUsers = new List<LicenseUser_Old>
            {
                new LicenseUser_Old {DisplayName = "John Doe", Id = Guid.Parse("594d8c64-37a9-4fa7-957c-2452653e32da")},
                new LicenseUser_Old {DisplayName = "Joe Scott", Id = Guid.Parse("36250b2f-7c56-4d07-9f01-4d2320139dc5")},
                new LicenseUser_Old {DisplayName = "Hello Fresh", Id = Guid.Parse("c832fb2d-bdf0-49fb-a299-1d5c1c6b5d75")},
                new LicenseUser_Old {DisplayName = "Jessica Remote", Id = Guid.Parse("33a320cb-51a2-4ab1-9d7e-81e593b2d1b7")},
                new LicenseUser_Old {DisplayName = "September Phone", Id = Guid.Parse("3dcbcd03-c726-4f60-a0a1-8c1a10be36a7")}
            };

            var apiUsers = new List<LicenseUser_Old>
            {
                new LicenseUser_Old {DisplayName = "John Doe", Id = Guid.Parse("594d8c64-37a9-4fa7-957c-2452653e32da")},
                new LicenseUser_Old {DisplayName = "Joe Scott", Id = Guid.Parse("36250b2f-7c56-4d07-9f01-4d2320139dc5")},
                new LicenseUser_Old {DisplayName = "Hello Fresh", Id = Guid.Parse("c832fb2d-bdf0-49fb-a299-1d5c1c6b5d75")},
                new LicenseUser_Old {DisplayName = "Jessica Remote", Id = Guid.Parse("33a320cb-51a2-4ab1-9d7e-81e593b2d1b7")},
                new LicenseUser_Old {DisplayName = "September Phone", Id = Guid.Parse("3dcbcd03-c726-4f60-a0a1-8c1a10be36a7")}
            };

            // act
            List<LicenseUser_Old> usersToUpdate = _userOrchestrator.ExposeGetUsersOrGroupsToUpdate<LicenseUser_Old, LicenseUserCompareLogic>(localUsers, apiUsers);

            // asset
            usersToUpdate.Count.ShouldBe(0);
        }

        [Fact]
        public void GetUsersOrGroupsToUpdate_ShouldReturnEmptyList_WhenSourceValuesAreNull()
        {
            // act
            List<LicenseUser_Old> usersToUpdate = _userOrchestrator.ExposeGetUsersOrGroupsToUpdate<LicenseUser_Old, LicenseUserCompareLogic>(null, null);

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

        public List<LicenseUser_Old> ExposeGetUsersOrGroupsToDelete(List<LicenseUser_Old> localUsers, List<LicenseUser_Old> apiUsers)
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