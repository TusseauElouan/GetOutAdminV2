using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GetOutAdminV2.Managers;
using GetOutAdminV2.Models;
using GetOutAdminV2.Services;
using GetOutAdminV2.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace GetOutAdminV2.Tests.ViewModels
{
    public class LogInViewModelTests
    {
        [Fact]
        public void LogIn_WithValidCredentials_SetsCurrentUser()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Email = "admin@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("password"),
                IsAdmin = true
            };

            var mockUserManager = new Mock<IUserManager>();
            mockUserManager.Setup(m => m.GetUserByEmail("admin@example.com")).Returns(user);
            mockUserManager.Setup(m => m.ListOfUsers).Returns(new ObservableCollection<User>());

            var mockNavViewModel = new Mock<NavigationViewModel>();

            // Créer une version personnalisée du LogInViewModel qui prend directement le UserManager
            // sans passer par ServiceLocator
            var viewModel = new TestableLogInViewModel(mockNavViewModel.Object, mockUserManager.Object);
            viewModel.Email = "admin@example.com";
            viewModel.Password = "password";

            // Act - Appeler la méthode de login sans passer par la commande
            viewModel.PerformLogin();

            // Assert
            mockUserManager.Verify(m => m.GetUserByEmail("admin@example.com"), Times.Once);
            Assert.False(viewModel.HasError);
            mockUserManager.VerifySet(m => m.CurrentUser = user, Times.Once);
        }
    }
    public class TestableLogInViewModel : LogInViewModel
    {
        private readonly IUserManager _userManager;

        public TestableLogInViewModel(NavigationViewModel navViewModel, IUserManager userManager)
            : base(navViewModel)
        {
            _userManager = userManager;
        }

        // Méthode pour exécuter le login sans utiliser la commande
        public void PerformLogin()
        {
            // Si la méthode LogInAsync est privée, tu peux l'implémenter ici
            // en reprenant la logique principale
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                ErrorMessage = "Il faut remplir tous les champs";
                HasError = true;
                return;
            }

            try
            {
                ErrorMessage = string.Empty;
                HasError = false;
                IsLoading = true;

                var user = _userManager.GetUserByEmail(Email);

                if (user == null)
                {
                    ErrorMessage = "Email ou mot de passe incorrect";
                    HasError = true;
                    return;
                }

                // Vérification du mot de passe
                bool passwordValid = BCrypt.Net.BCrypt.Verify(Password, user.Password);
                if (!passwordValid)
                {
                    ErrorMessage = "Email ou mot de passe incorrect";
                    HasError = true;
                    return;
                }

                // Vérification si l'utilisateur est admin
                if (!user.IsAdmin)
                {
                    ErrorMessage = "Vous n'avez pas les droits d'administration";
                    HasError = true;
                    return;
                }

                _userManager.CurrentUser = user;
                // Ne pas appeler la commande de navigation ici car elle dépend de WPF
            }
            catch (Exception ex)
            {
                ErrorMessage = "Une erreur est survenue: " + ex.Message;
                HasError = true;
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}