using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetOutAdminV2.Enum;
using GetOutAdminV2.Managers;
using GetOutAdminV2.Models;
using GetOutAdminV2.Services;
using GetOutAdminV2.Views;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace GetOutAdminV2.ViewModels
{
    public partial class ListUsersViewModel : BaseViewModel
    {
        private const int PageSize = 20; // Nombre d'utilisateurs par page
        private int _currentPage = -1; // Page actuelle (index backend)
        private int _totalUsers; // Nombre total d'utilisateurs

        private readonly IUserManager _userManager;

        public ObservableCollection<User> Users { get; }

        [ObservableProperty]
        private User? selectedUser;

        [ObservableProperty]
        private bool isDeletePopupOpen;

        [ObservableProperty]
        private bool canLoadPreviousPage;

        [ObservableProperty]
        private bool canLoadNextPage;

        [ObservableProperty]
        private string? pageInfo;

        [ObservableProperty]
        private int selectedPageIndex = 1;

        [ObservableProperty]
        private int totalPages;

        [ObservableProperty]
        private bool _isEditPopupOpen;

        [ObservableProperty]
        private bool? _hasError = false;

        [ObservableProperty]
        private string? _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _isPopupNotOpen;

        [ObservableProperty]
        private string _loadingVisibility = nameof(EVisibility.Hidden);

        [ObservableProperty]
        private string _dataGridVisibility = nameof(EVisibility.Visible);

        public IRelayCommand LoadNextPageCommand { get; }
        public IRelayCommand LoadPreviousPageCommand { get; }
        public IRelayCommand GoToPageCommand { get; }
        public IRelayCommand ShowDeletePopupCommand { get; }
        public IRelayCommand ShowEditPopupCommand { get; }
        public IRelayCommand ConfirmDeleteCommand { get; }
        public IRelayCommand CancelDeleteCommand { get; }
        public IRelayCommand CancelEditCommand { get; }
        public IRelayCommand ConfirmEditUserCommand { get; }

        // Propriété pour vérifier si un utilisateur est sélectionné
        public bool CanDeleteUser => SelectedUser != null;

        public ListUsersViewModel()
        {
            _userManager = ServiceLocator.GetRequiredService<IUserManager>();
            Users = new ObservableCollection<User>(_userManager.ListOfUsers);
            LoadNextPageCommand = new RelayCommand(LoadNextPage);
            LoadPreviousPageCommand = new RelayCommand(LoadPreviousPage);
            GoToPageCommand = new RelayCommand(GoToPage);

            // Commandes pour la gestion de la suppression
            ShowDeletePopupCommand = new RelayCommand(ShowDeletePopup, () => SelectedUser != null);
            ConfirmDeleteCommand = new RelayCommand(ConfirmDelete);
            CancelDeleteCommand = new RelayCommand(CancelDelete);

            // Commande pour la modification d'un utilisateur
            ShowEditPopupCommand = new RelayCommand(ShowEditPopup, () => SelectedUser != null);
            ConfirmEditUserCommand = new RelayCommand(ConfirmEditUser);
            CancelEditCommand = new RelayCommand(CancelEdit);

            _totalUsers = _userManager.ListOfUsers.Count;
            TotalPages = (int)Math.Ceiling((double)_totalUsers / PageSize);
            LoadNextPage();
        }

        // Méthode exécutée lorsque SelectedUser change
        partial void OnSelectedUserChanged(User? oldValue, User? newValue)
        {
            // Notifier les commandes de vérifier à nouveau leurs conditions d'activation
            (ShowDeletePopupCommand as RelayCommand)?.NotifyCanExecuteChanged();
            (ShowEditPopupCommand as RelayCommand)?.NotifyCanExecuteChanged();
        }

        private void ShowDeletePopup()
        {
            if (IsDeletePopupOpen || SelectedUser == null) return;
            IsPopupNotOpen = false;
            IsDeletePopupOpen = true;
        }

        private void ShowEditPopup()
        {
            if (IsEditPopupOpen || SelectedUser == null) return;
            IsPopupNotOpen = false;
            IsEditPopupOpen = true;
        }

        private void ConfirmDelete()
        {
            if (SelectedUser == null) return;

            LoadingVisibility = nameof(EVisibility.Visible);
            DataGridVisibility = nameof(EVisibility.Hidden);
            _userManager.DeleteUser(SelectedUser.Id);
            Users.Remove(SelectedUser);
            SelectedUser = null;
            IsDeletePopupOpen = false;

            LoadingVisibility = nameof(EVisibility.Hidden);
            DataGridVisibility = nameof(EVisibility.Visible);

            _totalUsers = _userManager.ListOfUsers.Count;
            TotalPages = (int)Math.Ceiling((double)_totalUsers / PageSize);
            LoadUsersForPage(_currentPage);
        }

        private void CancelDelete()
        {
            IsDeletePopupOpen = false;
        }

        private void CancelEdit()
        {
            if (SelectedUser == null) return;
            LoadingVisibility = nameof(EVisibility.Visible);
            DataGridVisibility = nameof(EVisibility.Hidden);
            User originalUser = _userManager.GetUserById(SelectedUser.Id);
            SelectedUser.Nom = originalUser.Nom;
            SelectedUser.Prenom = originalUser.Prenom;
            SelectedUser.Tag = originalUser.Tag;

            LoadingVisibility = nameof(EVisibility.Hidden);
            DataGridVisibility = nameof(EVisibility.Visible);

            LoadUsersForPage(_currentPage);

            IsEditPopupOpen = false;
        }

        private void ConfirmEditUser()
        {
            if (SelectedUser == null) return;

            try
            {
                if (string.IsNullOrWhiteSpace(SelectedUser.Nom) ||
                    string.IsNullOrWhiteSpace(SelectedUser.Prenom) ||
                    string.IsNullOrWhiteSpace(SelectedUser.Tag))
                {
                    ErrorMessage = "Tous les champs sont obligatoires.";
                    HasError = true;
                    return;
                }

                // Mettre à jour l'utilisateur
                LoadingVisibility = nameof(EVisibility.Visible);
                DataGridVisibility = nameof(EVisibility.Hidden);
                _userManager.UpdateUser(SelectedUser);

                ErrorMessage = "Utilisateur mis à jour avec succès.";
                LoadingVisibility = nameof(EVisibility.Hidden);
                DataGridVisibility = nameof(EVisibility.Visible);
                HasError = false;  // Pas d'erreur
                IsEditPopupOpen = false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur lors de la sauvegarde: {ex.Message}";
                HasError = true;
            }
        }

        private IEnumerable<User> GetUsers(int startIndex, int count) => _userManager.ListOfUsers.Skip(startIndex).Take(count);

        private void LoadNextPage()
        {
            _currentPage++;
            LoadUsersForPage(_currentPage);
            SelectedPageIndex = _currentPage + 1;
            CanLoadPreviousPage = _currentPage > 0;
            CanLoadNextPage = _totalUsers > (_currentPage + 1) * PageSize;
        }

        private void LoadPreviousPage()
        {
            _currentPage--;
            LoadUsersForPage(_currentPage);
            SelectedPageIndex = _currentPage + 1;
            CanLoadPreviousPage = _currentPage > 0;
            CanLoadNextPage = _totalUsers > (_currentPage + 1) * PageSize;
        }

        private void GoToPage()
        {
            int backendPageIndex = SelectedPageIndex - 1;
            int maxPageIndex = (int)Math.Ceiling((double)_totalUsers / PageSize) - 1;

            if (backendPageIndex < 0 || backendPageIndex > maxPageIndex)
            {
                MessageBox.Show($"Index de page invalide. Veuillez saisir un index entre 1 et {maxPageIndex + 1}.");
                SelectedPageIndex = _currentPage + 1;
                return;
            }

            _currentPage = backendPageIndex;
            LoadUsersForPage(_currentPage);
        }

        private void LoadUsersForPage(int pageIndex)
        {
            var users = GetUsers(pageIndex * PageSize, PageSize);

            Users.Clear();
            foreach (var user in users)
            {
                Users.Add(user);
            }

            CanLoadPreviousPage = _currentPage > 0;
            CanLoadNextPage = _totalUsers > (_currentPage + 1) * PageSize;
        }
    }
}