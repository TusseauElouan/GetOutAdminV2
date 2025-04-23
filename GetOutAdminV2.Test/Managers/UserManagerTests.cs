using System;
using System.Collections.Generic;
using System.Linq;
using GetOutAdminV2.Managers;
using GetOutAdminV2.Models;
using GetOutAdminV2.Providers;
using Moq;
using Xunit;

namespace GetOutAdminV2.Tests.Managers
{
    public class UserManagerTests
    {
        [Fact]
        public void GetUserById_ReturnsCorrectUser()
        {
            // Arrange
            var expectedUser = new User { Id = 1, Nom = "Doe", Prenom = "John", Email = "john.doe@example.com" };

            var mockUserProvider = new Mock<IUserProvider>();
            mockUserProvider.Setup(provider => provider.GetUserById(1)).Returns(expectedUser);

            var userManager = new UserManager(mockUserProvider.Object);

            // Act
            var result = userManager.GetUserById(1);

            // Assert
            Assert.Equal(expectedUser, result);
        }
    }
}