using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetOutAdminV2.Managers;
using GetOutAdminV2.Models;
using GetOutAdminV2.Services;
using GetOutAdminV2.Views;
using System;
using System.Windows;
using System.Windows.Navigation;

namespace GetOutAdminV2.ViewModels
{
    public partial class EditUserViewModel : BaseViewModel
    {
        private readonly IUserManager _userManager;
        private long _userId;

        [ObservableProperty]
        private User user;

        public IRelayCommand SaveCommand { get; }
        public IRelayCommand CancelCommand { get; }

        public EditUserViewModel(long userId)
        {
            _userManager = ServiceLocator.GetRequiredService<IUserManager>();

            SaveCommand = new RelayCommand(SaveUser);
            CancelCommand = new RelayCommand(Cancel);

            _userManager = ServiceLocator.GetRequiredService<IUserManager>();
            _userId = userId;
            User = _userManager.GetUserById(_userId);
        }

        public void Initialize(object parameter)
        {
            if (parameter is int userId)
            {
                _userId = userId;
                LoadUser(userId);
            }
        }

        private void LoadUser(int userId)
        {
            try
            {
                var user = _userManager.GetUserById(userId);
                if (user != null)
                {
                    // Créer une copie de l'utilisateur pour éviter de modifier l'original
                    User = new User
                    {
                        Id = user.Id,
                        Nom = user.Nom,
                        Prenom = user.Prenom,
                        Tag = user.Tag
                    };
                }
                else
                {
                    MessageBox.Show("Utilisateur non trouvé.");
                    Cancel();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement de l'utilisateur: {ex.Message}");
                Cancel();
            }
        }

        private void SaveUser()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(User.Nom) ||
                    string.IsNullOrWhiteSpace(User.Prenom) ||
                    string.IsNullOrWhiteSpace(User.Tag))
                {
                    MessageBox.Show("Tous les champs sont obligatoires.");
                    return;
                }

                // Mettre à jour l'utilisateur
                _userManager.UpdateUser(User);

                MessageBox.Show("Utilisateur mis à jour avec succès.");

                // Retourner à la liste des utilisateurs
                CurrentPage = new ListUserPage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la sauvegarde: {ex.Message}");
            }
        }

        private void Cancel()
        {
            CurrentPage = new ListUserPage();
        }
    }
}