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
        private readonly ISanctionManager _sanctionManager;
        private readonly ITypeReportManager _typeReportManager;

        public ObservableCollection<User> Users { get; }

        [ObservableProperty]
        private User? selectedUser;

        [ObservableProperty]
        private bool isSanctionPopupOpen;

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

        [ObservableProperty]
        private ESanctionDuration _selectedSanctionDuration = ESanctionDuration.TwoWeeks;

        [ObservableProperty]
        private string _sanctionDescription = string.Empty;

        [ObservableProperty]
        private string _activeSanctionInfo = string.Empty;

        [ObservableProperty]
        private bool _hasActiveSanction = false;

        public IEnumerable<ESanctionDuration> SanctionDurations => System.Enum.GetValues(typeof(ESanctionDuration)).Cast<ESanctionDuration>();

        public IRelayCommand LoadNextPageCommand { get; }
        public IRelayCommand LoadPreviousPageCommand { get; }
        public IRelayCommand GoToPageCommand { get; }
        public IRelayCommand ShowSanctionPopupCommand { get; }
        public IRelayCommand ShowEditPopupCommand { get; }
        public IRelayCommand ConfirmSanctionCommand { get; }
        public IRelayCommand CancelSanctionCommand { get; }
        public IRelayCommand CancelEditCommand { get; }
        public IRelayCommand ConfirmEditUserCommand { get; }
        public IRelayCommand PromoteToAdminCommand { get; }

        // Propriété pour vérifier si un utilisateur est sélectionné
        public bool CanSanctionUser => SelectedUser != null;

        public ListUsersViewModel()
        {
            _userManager = ServiceLocator.GetRequiredService<IUserManager>();
            _sanctionManager = ServiceLocator.GetRequiredService<ISanctionManager>();
            _typeReportManager = ServiceLocator.GetRequiredService<ITypeReportManager>();

            Users = new ObservableCollection<User>(_userManager.ListOfUsers);
            LoadNextPageCommand = new RelayCommand(LoadNextPage);
            LoadPreviousPageCommand = new RelayCommand(LoadPreviousPage);
            GoToPageCommand = new RelayCommand(GoToPage);

            // Commandes pour la gestion des sanctions
            ShowSanctionPopupCommand = new RelayCommand(ShowSanctionPopup, () => SelectedUser != null);
            ConfirmSanctionCommand = new RelayCommand(ConfirmSanction);
            CancelSanctionCommand = new RelayCommand(CancelSanction);

            // Commande pour la modification d'un utilisateur
            ShowEditPopupCommand = new RelayCommand(ShowEditPopup, () => SelectedUser != null);
            ConfirmEditUserCommand = new RelayCommand(ConfirmEditUser);
            CancelEditCommand = new RelayCommand(CancelEdit);

            PromoteToAdminCommand = new RelayCommand(PromoteToAdmin, () => SelectedUser != null && !SelectedUser.IsAdmin);

            _totalUsers = _userManager.ListOfUsers.Count;
            TotalPages = (int)Math.Ceiling((double)_totalUsers / PageSize);

            LoadUsersExcludingAdmins();

            LoadNextPage();
        }
        private void PromoteToAdmin()
        {
            if (SelectedUser == null) return;

            try
            {
                LoadingVisibility = nameof(EVisibility.Visible);
                DataGridVisibility = nameof(EVisibility.Hidden);

                // Demander confirmation
                var result = MessageBox.Show(
                    $"Êtes-vous sûr de vouloir promouvoir {SelectedUser.Prenom} {SelectedUser.Nom} au statut d'administrateur ?",
                    "Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    SelectedUser.IsAdmin = true;
                    _userManager.UpdateUser(SelectedUser);

                    MessageBox.Show(
                        $"{SelectedUser.Prenom} {SelectedUser.Nom} a été promu administrateur avec succès.",
                        "Promotion réussie",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    // Recharger la liste des utilisateurs sans les admins
                    LoadUsersExcludingAdmins();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la promotion de l'utilisateur : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                LoadingVisibility = nameof(EVisibility.Hidden);
                DataGridVisibility = nameof(EVisibility.Visible);
            }
        }
        // Méthode exécutée lorsque SelectedUser change
        partial void OnSelectedUserChanged(User? oldValue, User? newValue)
        {
            // Notifier les commandes
            (ShowSanctionPopupCommand as RelayCommand)?.NotifyCanExecuteChanged();
            (ShowEditPopupCommand as RelayCommand)?.NotifyCanExecuteChanged();
            (PromoteToAdminCommand as RelayCommand)?.NotifyCanExecuteChanged();

            // Vérifier si l'utilisateur a une sanction active
            if (newValue != null)
            {
                CheckActiveSanction(newValue.Id);
            }
            else
            {
                HasActiveSanction = false;
                ActiveSanctionInfo = string.Empty;
            }
        }
        private void CheckActiveSanction(long userId)
        {
            var activeSanction = _sanctionManager.GetActiveSanctionByUserId(userId);
            if (activeSanction != null)
            {
                HasActiveSanction = true;

                // Récupérer le type de sanction
                string typeName = "Inconnu";
                if (activeSanction.TypeReportUsers == null && activeSanction.TypeReportUsersId > 0)
                {
                    var typeReport = _typeReportManager.GetTypeReportById(activeSanction.TypeReportUsersId);
                    if (typeReport != null)
                    {
                        typeName = typeReport.Name;
                    }
                }
                else if (activeSanction.TypeReportUsers != null)
                {
                    typeName = activeSanction.TypeReportUsers.Name;
                }

                string endDate = activeSanction.IsPermanent ? "permanente" : $"jusqu'au {activeSanction.EndAt?.ToString("dd/MM/yyyy")}";
                ActiveSanctionInfo = $"Type: {typeName} - {endDate}";
            }
            else
            {
                HasActiveSanction = false;
                ActiveSanctionInfo = string.Empty;
            }
        }
        private void ShowSanctionPopup()
        {
            if (IsSanctionPopupOpen || SelectedUser == null) return;
            IsPopupNotOpen = false;

            // Vérifier si l'utilisateur a déjà une sanction active
            var activeSanction = _sanctionManager.GetActiveSanctionByUserId(SelectedUser.Id);
            if (activeSanction != null)
            {
                string endDate = activeSanction.IsPermanent ? "permanente" : $"jusqu'au {activeSanction.EndAt?.ToString("dd/MM/yyyy")}";
                MessageBox.Show($"Cet utilisateur a déjà une sanction active {endDate}.", "Sanction existante", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Réinitialiser les valeurs
            SelectedSanctionDuration = ESanctionDuration.TwoWeeks;
            SanctionDescription = string.Empty;

            IsSanctionPopupOpen = true;
        }

        private void ShowEditPopup()
        {
            if (IsEditPopupOpen || SelectedUser == null) return;
            IsPopupNotOpen = false;
            IsEditPopupOpen = true;
        }

        private void ConfirmSanction()
        {
            if (SelectedUser == null) return;

            try
            {
                LoadingVisibility = nameof(EVisibility.Visible);
                DataGridVisibility = nameof(EVisibility.Hidden);

                // Créer la nouvelle sanction
                var now = DateTime.Now;

                // Trouver un type de rapport approprié (nous utilisons le premier par défaut)
                var typeReport = _typeReportManager.ListOfTypeReports.FirstOrDefault();
                if (typeReport == null)
                {
                    // Si aucun type n'existe, charger les types
                    _typeReportManager.GetAllTypeReports();
                    typeReport = _typeReportManager.ListOfTypeReports.FirstOrDefault();

                    if (typeReport == null)
                    {
                        MessageBox.Show("Aucun type de rapport trouvé dans la base de données.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                var sanction = new SanctionsUser
                {
                    UserId = SelectedUser.Id,
                    TypeReportUsersId = typeReport.Id,
                    Description = SanctionDescription,
                    Status = "active",
                    StartAt = now,
                    CreatedAt = now,
                    UpdatedAt = now,
                    IsPermanent = SelectedSanctionDuration == ESanctionDuration.Permanent
                };

                // Calculer la date de fin en fonction de la durée sélectionnée
                if (SelectedSanctionDuration != ESanctionDuration.Permanent)
                {
                    sanction.EndAt = SelectedSanctionDuration.CalculateEndDate(now);
                }

                _sanctionManager.AddSanction(sanction);

                MessageBox.Show($"L'utilisateur {SelectedUser.Prenom} {SelectedUser.Nom} a été sanctionné avec succès.", "Sanction appliquée", MessageBoxButton.OK, MessageBoxImage.Information);

                IsSanctionPopupOpen = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'application de la sanction : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                LoadingVisibility = nameof(EVisibility.Hidden);
                DataGridVisibility = nameof(EVisibility.Visible);
            }
        }

        private void CancelSanction()
        {
            IsSanctionPopupOpen = false;
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

        private IEnumerable<User> GetUsers(int startIndex, int count)
        {
            return _userManager.ListOfUsers
                .Where(u => !u.IsAdmin)
                .Skip(startIndex)
                .Take(count);
        }

        private void LoadUsersExcludingAdmins()
        {
            try
            {
                LoadingVisibility = nameof(EVisibility.Visible);
                DataGridVisibility = nameof(EVisibility.Hidden);

                _userManager.GetAllUsers();

                // Compter uniquement les utilisateurs non-admins
                _totalUsers = _userManager.ListOfUsers.Count(u => !u.IsAdmin);
                TotalPages = (int)Math.Ceiling((double)_totalUsers / PageSize);

                // Réinitialiser la page actuelle
                _currentPage = 0;
                LoadUsersForPage(_currentPage);
                SelectedPageIndex = _currentPage + 1;

                // Mettre à jour les boutons de navigation
                CanLoadPreviousPage = _currentPage > 0;
                CanLoadNextPage = _totalUsers > (_currentPage + 1) * PageSize;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des utilisateurs : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                LoadingVisibility = nameof(EVisibility.Hidden);
                DataGridVisibility = nameof(EVisibility.Visible);
            }
        }

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