using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GetOutAdminV2.Managers;
using GetOutAdminV2.Models;
using GetOutAdminV2.Services;
using GetOutAdminV2.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace GetOutAdminV2.Tests.ViewModels
{
    public class ListUsersViewModelTests
    {
        private Mock<IUserManager> SetupUserManager()
        {
            var users = new List<User>
            {
                new User { Id = 1, Nom = "User1", Prenom = "Test", IsAdmin = false },
                new User { Id = 2, Nom = "User2", Prenom = "Test", IsAdmin = false },
                new User { Id = 3, Nom = "User3", Prenom = "Test", IsAdmin = false },
                new User { Id = 4, Nom = "User4", Prenom = "Test", IsAdmin = false },
                new User { Id = 5, Nom = "Admin", Prenom = "Admin", IsAdmin = true }
            };

            var mockUserManager = new Mock<IUserManager>();
            mockUserManager.Setup(m => m.ListOfUsers).Returns(new ObservableCollection<User>(users));
            mockUserManager.Setup(m => m.GetUserById(It.IsAny<long>())).Returns<long>(id => users.FirstOrDefault(u => u.Id == id));
            return mockUserManager;
        }

        private void SetupServiceLocator(Mock<IUserManager> mockUserManager, Mock<ISanctionManager> mockSanctionManager, Mock<ITypeReportManager> mockTypeReportManager)
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(sp => sp.GetService(typeof(IUserManager))).Returns(mockUserManager.Object);
            serviceProvider.Setup(sp => sp.GetService(typeof(ISanctionManager))).Returns(mockSanctionManager.Object);
            serviceProvider.Setup(sp => sp.GetService(typeof(ITypeReportManager))).Returns(mockTypeReportManager.Object);

            ServiceLocator.Initialize(serviceProvider.Object);
        }

        [Fact]
        public void Constructor_InitializesWithCorrectData()
        {
            // Arrange
            var mockUserManager = SetupUserManager();
            var mockSanctionManager = new Mock<ISanctionManager>();
            var mockTypeReportManager = new Mock<ITypeReportManager>();

            SetupServiceLocator(mockUserManager, mockSanctionManager, mockTypeReportManager);

            // Act
            var viewModel = new ListUsersViewModel();

            // Assert
            Assert.Equal(1, viewModel.SelectedPageIndex);
            Assert.True(viewModel.Users.All(u => !u.IsAdmin));
            Assert.Equal(4, viewModel.Users.Count); // 4 non-admin users should be displayed
        }

        [Fact]
        public void LoadNextPage_UpdatesCurrentPage()
        {
            // Arrange
            var mockUserManager = SetupUserManager();
            var mockSanctionManager = new Mock<ISanctionManager>();
            var mockTypeReportManager = new Mock<ITypeReportManager>();

            SetupServiceLocator(mockUserManager, mockSanctionManager, mockTypeReportManager);

            var viewModel = new ListUsersViewModel();
            var initialPageIndex = viewModel.SelectedPageIndex;

            // Act
            viewModel.LoadNextPageCommand.Execute(null);

            // Assert
            Assert.Equal(initialPageIndex + 1, viewModel.SelectedPageIndex);
        }
    }
}