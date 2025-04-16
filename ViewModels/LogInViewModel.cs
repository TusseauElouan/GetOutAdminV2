using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetOutAdminV2.Managers;
using GetOutAdminV2.Models;
using GetOutAdminV2.Services;
using GetOutAdminV2.Views;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Input;

namespace GetOutAdminV2.ViewModels
{
    public partial class LogInViewModel : BaseViewModel
    {
        private readonly IUserManager _userManager;
        private readonly NavigationViewModel _navigationViewModel;
        public LogInViewModel(NavigationViewModel navigationViewModel)
        {
            Email = string.Empty;
            Password = string.Empty;
            HasError = false;
            ErrorMessage = string.Empty;
            IsLoading = false;

            _navigationViewModel = navigationViewModel;

            _userManager = ServiceLocator.GetRequiredService<IUserManager>();
            _userManager.GetAllUsers();
        }

        [ObservableProperty]
        private bool? _hasError = false;

        [ObservableProperty]
        private string? _errorMessage = string.Empty;

        // Modifiez la méthode LogIn pour utiliser async/await
        private ICommand? _logInCommand;
        public ICommand LogInCommand => _logInCommand ??= new AsyncRelayCommand(LogInAsync);

        private async Task LogInAsync()
        {
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

                // Exécutez l'opération de base de données sur un thread en arrière-plan
                var user = await Task.Run(() => _userManager.GetUserByEmail(Email));

                if (user == null)
                {
                    ErrorMessage = "Email ou mot de passe incorrect";
                    HasError = true;
                    return;
                }

                // Vérifiez le mot de passe de manière asynchrone
                bool passwordValid = await Task.Run(() => VerifyPassword(Password, user.Password));
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
                _navigationViewModel.DashBoardCommand.Execute(null);
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
        private bool VerifyPassword(string password, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }
    }
}
