using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetOutAdminV2.Managers;
using GetOutAdminV2.Models;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Input;

namespace GetOutAdminV2.ViewModels
{
    public partial class LogInViewModel : BaseViewModel
    {
        private readonly IUserManager _userManager;
        public LogInViewModel(IUserManager userManager)
        {
            Email = string.Empty;
            Password = string.Empty;
            HasError = false;
            ErrorMessage = string.Empty;
            IsLoading = false;

            _userManager = userManager;
        }

        public LogInViewModel() { }

        [ObservableProperty]
        private bool? _hasError; 

        [ObservableProperty]
        private string? _errorMessage;

        private ICommand? _logInCommand;
        public ICommand LogInCommand => _logInCommand ??= new AsyncRelayCommand(LogInAsync); // Notez le AsyncRelayCommand

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

                string hashedPassword = HashPassword(Password);
                await Task.Delay(TimeSpan.FromSeconds(2)); // Maintenant le delay sera effectif

                var user = new User
                {
                    Email = Email,
                    Password = hashedPassword
                };

                _userManager.CurrentUser = user; // Pas besoin de créer un nouveau User vide avant
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

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
