using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseMonitoringSystem.Tests.Core
{
    using global::Core.Common.Extensions;
    using global::Core.Users;
    using Shouldly;
    using Xunit;

    public class Extension_Tests
    {
        [Fact]
        public void FilterMissing_ShouldReturnFilteredList_WhenSourceIsNotEmpty()
        {
            // Arrange
            var existingUsers = new List<User>
            {
                new User {Id = Guid.Parse("1f14172a-0869-41c3-823f-28f605c83d8c"), DisplayName = "Thomas Adams"},
                new User {Id = Guid.Parse("762e1f40-2a78-4912-a01d-fdc2c12fb6d6"), DisplayName = "Adam Jacques"},
            };


            var allUsers = new List<User>
            {
                new User {Id = Guid.Parse("1f14172a-0869-41c3-823f-28f605c83d8c"), DisplayName = "Thomas Adams"},
                new User {Id = Guid.Parse("762e1f40-2a78-4912-a01d-fdc2c12fb6d6"), DisplayName = "Adam Jacques"},
                new User {Id = Guid.Parse("3886b206-902a-49b2-aa65-8bce88b3b23b"), DisplayName = "Andy Summerfield"},
                new User {Id = Guid.Parse("f25b3a1f-cedc-4ba2-be56-733aab5bf51b"), DisplayName = "Adam Collin"},
                new User {Id = Guid.Parse("8b4567f5-d782-48eb-aa5e-01199bce606a"), DisplayName = "Chris Barr"},
            };



            // Act
            var createUsers = allUsers.FilterMissing<User,Guid>(existingUsers);

            // Assert
            createUsers.Count.ShouldBe(3);

            createUsers.ShouldBe(new List<User>
            {
                                new User {Id = Guid.Parse("3886b206-902a-49b2-aa65-8bce88b3b23b"), DisplayName = "Andy Summerfield"},
                new User {Id = Guid.Parse("f25b3a1f-cedc-4ba2-be56-733aab5bf51b"), DisplayName = "Adam Collin"},
                new User {Id = Guid.Parse("8b4567f5-d782-48eb-aa5e-01199bce606a"), DisplayName = "Chris Barr"}
            });


        }

        [Fact]
        public void FilterExisting_ShouldReturnFilteredList_WhenSourceIsNotEmpty()
        {
            // Arrange
            var existingUsers = new List<User>
            {
                new User {Id = Guid.Parse("1f14172a-0869-41c3-823f-28f605c83d8c"), DisplayName = "Thomas Adams"},
                new User {Id = Guid.Parse("762e1f40-2a78-4912-a01d-fdc2c12fb6d6"), DisplayName = "Adam Jacques"},
            };


            var allUsers = new List<User>
            {
                new User {Id = Guid.Parse("1f14172a-0869-41c3-823f-28f605c83d8c"), DisplayName = "Thomas Adams"},
                new User {Id = Guid.Parse("762e1f40-2a78-4912-a01d-fdc2c12fb6d6"), DisplayName = "Adam Jacques"},
                new User {Id = Guid.Parse("3886b206-902a-49b2-aa65-8bce88b3b23b"), DisplayName = "Andy Summerfield"},
                new User {Id = Guid.Parse("f25b3a1f-cedc-4ba2-be56-733aab5bf51b"), DisplayName = "Adam Collin"},
                new User {Id = Guid.Parse("8b4567f5-d782-48eb-aa5e-01199bce606a"), DisplayName = "Chris Barr"},
            };



            // Act
            var createUsers = allUsers.FilterExisting<User, Guid>(existingUsers);

            // Assert
            createUsers.Count.ShouldBe(2);

            createUsers.ShouldBe(new List<User>
            {
                new User {Id = Guid.Parse("1f14172a-0869-41c3-823f-28f605c83d8c"), DisplayName = "Thomas Adams"},
                new User {Id = Guid.Parse("762e1f40-2a78-4912-a01d-fdc2c12fb6d6"), DisplayName = "Adam Jacques"},
            });


        }

        [Fact]
        public void FilterMissing_ShouldReturnEmptyList_WhenSourceIsEmpty()
        {
            // Arrange
            var existingUsers = new List<User>
            {
                new User {Id = Guid.Parse("1f14172a-0869-41c3-823f-28f605c83d8c"), DisplayName = "Thomas Adams"},
                new User {Id = Guid.Parse("762e1f40-2a78-4912-a01d-fdc2c12fb6d6"), DisplayName = "Adam Jacques"},
            };


            var allUsers = new List<User>
            {

            };


            // Act
            var createUsers = allUsers.FilterMissing<User, Guid>(existingUsers);

            // Assert
            createUsers.Count.ShouldBe(0);
        }

        [Fact]
        public void FilterExisting_ShouldReturnEmptyList_WhenSourceIsEmpty()
        {
            // Arrange
            var existingUsers = new List<User>
            {
                new User {Id = Guid.Parse("1f14172a-0869-41c3-823f-28f605c83d8c"), DisplayName = "Thomas Adams"},
                new User {Id = Guid.Parse("762e1f40-2a78-4912-a01d-fdc2c12fb6d6"), DisplayName = "Adam Jacques"},
            };


            var allUsers = new List<User>
            {

            };


            // Act
            var createUsers = allUsers.FilterExisting<User, Guid>(existingUsers);

            // Assert
            createUsers.Count.ShouldBe(0);
        }

    }
}
