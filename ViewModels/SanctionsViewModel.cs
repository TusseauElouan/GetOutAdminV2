using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetOutAdminV2.Enum;
using GetOutAdminV2.Managers;
using GetOutAdminV2.Models;
using GetOutAdminV2.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GetOutAdminV2.ViewModels
{
    public partial class SanctionsViewModel : BaseViewModel
    {
        private const int PageSize = 20; // Nombre de sanctions par page
        private int _currentPage = -1; // Page actuelle (index backend)
        private int _totalSanctions; // Nombre total de sanctions

        private readonly ISanctionManager _sanctionManager;
        private readonly IUserManager _userManager;
        private readonly ITypeReportManager _typeReportManager;

        public ObservableCollection<SanctionsUser> Sanctions { get; }

        [ObservableProperty]
        private SanctionsUser? selectedSanction;

        [ObservableProperty]
        private bool canLoadPreviousPage;

        [ObservableProperty]
        private bool canLoadNextPage;

        [ObservableProperty]
        private int selectedPageIndex = 1;

        [ObservableProperty]
        private int totalPages;

        [ObservableProperty]
        private string _loadingVisibility = nameof(EVisibility.Hidden);

        [ObservableProperty]
        private string _dataGridVisibility = nameof(EVisibility.Visible);

        [ObservableProperty]
        private bool _isConfirmCancelPopupOpen = false;

        [ObservableProperty]
        private string _cancelReason = string.Empty;

        public IRelayCommand LoadNextPageCommand { get; }
        public IRelayCommand LoadPreviousPageCommand { get; }
        public IRelayCommand GoToPageCommand { get; }
        public IRelayCommand CancelSanctionCommand { get; }
        public IRelayCommand ConfirmCancelSanctionCommand { get; }
        public IRelayCommand CloseCancelPopupCommand { get; }

        public SanctionsViewModel()
        {
            _sanctionManager = ServiceLocator.GetRequiredService<ISanctionManager>();
            _userManager = ServiceLocator.GetRequiredService<IUserManager>();
            _typeReportManager = ServiceLocator.GetRequiredService<ITypeReportManager>();

            Sanctions = new ObservableCollection<SanctionsUser>();

            LoadNextPageCommand = new RelayCommand(LoadNextPage);
            LoadPreviousPageCommand = new RelayCommand(LoadPreviousPage);
            GoToPageCommand = new RelayCommand(GoToPage);

            // Nouvelles commandes pour annuler des sanctions
            CancelSanctionCommand = new RelayCommand(ShowCancelSanctionPopup, () => CanCancelSanction);
            ConfirmCancelSanctionCommand = new RelayCommand(ConfirmCancelSanction);
            CloseCancelPopupCommand = new RelayCommand(CloseCancelPopup);

            LoadSanctions();
        }

        // Propriété pour vérifier si la sanction sélectionnée peut être annulée
        public bool CanCancelSanction => SelectedSanction != null && SelectedSanction.Status == "active";

        private void ShowCancelSanctionPopup()
        {
            if (SelectedSanction == null || SelectedSanction.Status != "active")
            {
                NotificationService.Notify("Sélectionnez une sanction active pour l'annuler.", NotificationType.Warning);
                return;
            }

            CancelReason = string.Empty;
            IsConfirmCancelPopupOpen = true;
        }

        private void CloseCancelPopup()
        {
            IsConfirmCancelPopupOpen = false;
        }

        private void ConfirmCancelSanction()
        {
            if (SelectedSanction == null) return;

            try
            {
                LoadingVisibility = nameof(EVisibility.Visible);
                DataGridVisibility = nameof(EVisibility.Hidden);

                // Récupérer la sanction depuis la base de données pour être sûr qu'elle est bien trackée
                var sanctionToUpdate = _sanctionManager.GetSanctionById(SelectedSanction.Id);

                if (sanctionToUpdate == null)
                {
                    NotificationService.Notify("Impossible de trouver la sanction à annuler.", NotificationType.Error);
                    return;
                }

                // Mettre à jour le statut de la sanction
                sanctionToUpdate.Status = "inactive"; // Utiliser "inactive" au lieu de "canceled"
                sanctionToUpdate.UpdatedAt = DateTime.Now;

                // Ajouter un champ pour la raison d'annulation dans la description
                if (string.IsNullOrEmpty(sanctionToUpdate.Description))
                {
                    sanctionToUpdate.Description = $"[ANNULÉ LE {DateTime.Now:dd/MM/yyyy}] Raison: {CancelReason}";
                }
                else
                {
                    sanctionToUpdate.Description += $"\n[ANNULÉ LE {DateTime.Now:dd/MM/yyyy}] Raison: {CancelReason}";
                }

                // Mettre à jour la sanction
                _sanctionManager.UpdateSanction(sanctionToUpdate);

                NotificationService.Notify("La sanction a été annulée avec succès.", NotificationType.Success);
                IsConfirmCancelPopupOpen = false;

                // Recharger les sanctions
                SelectedSanction = null;
                _sanctionManager.GetAllSanctions(); // Recharger depuis la base de données
                LoadSanctions();
            }
            catch (Exception ex)
            {
                NotificationService.Notify($"Erreur lors de l'annulation de la sanction : {ex.Message}", NotificationType.Error);
            }
            finally
            {
                LoadingVisibility = nameof(EVisibility.Hidden);
                DataGridVisibility = nameof(EVisibility.Visible);
            }
        }

        // Méthode exécutée lorsque SelectedSanction change
        partial void OnSelectedSanctionChanged(SanctionsUser? oldValue, SanctionsUser? newValue)
        {
            // Notifier les commandes 
            (CancelSanctionCommand as RelayCommand)?.NotifyCanExecuteChanged();
        }

        private void LoadSanctions()
        {
            try
            {
                LoadingVisibility = nameof(EVisibility.Visible);
                DataGridVisibility = nameof(EVisibility.Hidden);

                // Charger toutes les sanctions
                _sanctionManager.GetAllSanctions();

                // Filtrer pour n'afficher que les sanctions actives
                var now = DateTime.Now;
                var activeSanctions = _sanctionManager.ListOfSanctions
                    .Where(s => s.Status == "active" && (s.IsPermanent || s.EndAt > now))
                    .OrderByDescending(s => s.CreatedAt)
                    .ToList();

                _totalSanctions = activeSanctions.Count;
                TotalPages = (int)Math.Ceiling((double)_totalSanctions / PageSize);

                // Si aucune sanction active, afficher un message
                if (_totalSanctions == 0)
                {
                    NotificationService.Notify("Aucune sanction active n'a été trouvée.", NotificationType.Info);
                }

                // Réinitialiser le compteur de page à 0 si aucune sanction n'est disponible
                if (_totalSanctions == 0)
                {
                    _currentPage = 0;
                }
                // S'assurer que le numéro de page actuel est valide
                else if (_currentPage < 0 || _currentPage >= TotalPages)
                {
                    _currentPage = 0;
                }

                LoadSanctionsForPage(_currentPage, activeSanctions);
            }
            catch (Exception ex)
            {
                NotificationService.Notify($"Erreur lors du chargement des sanctions : {ex.Message}", NotificationType.Error);
            }
            finally
            {
                LoadingVisibility = nameof(EVisibility.Hidden);
                DataGridVisibility = nameof(EVisibility.Visible);
            }
        }

        private IEnumerable<SanctionsUser> GetSanctions(int startIndex, int count, List<SanctionsUser> sanctions)
        {
            if (startIndex >= sanctions.Count)
            {
                return new List<SanctionsUser>();
            }
            return sanctions.Skip(startIndex).Take(count);
        }

        private void LoadNextPage()
        {
            _currentPage++;

            // Filtrer pour n'afficher que les sanctions actives
            var now = DateTime.Now;
            var activeSanctions = _sanctionManager.ListOfSanctions
                .Where(s => s.Status == "active" && (s.IsPermanent || s.EndAt > now))
                .OrderByDescending(s => s.CreatedAt)
                .ToList();

            LoadSanctionsForPage(_currentPage, activeSanctions);
            SelectedPageIndex = _currentPage + 1;
            CanLoadPreviousPage = _currentPage > 0;
            CanLoadNextPage = _totalSanctions > (_currentPage + 1) * PageSize;
        }

        private void LoadPreviousPage()
        {
            _currentPage--;

            // Filtrer pour n'afficher que les sanctions actives
            var now = DateTime.Now;
            var activeSanctions = _sanctionManager.ListOfSanctions
                .Where(s => s.Status == "active" && (s.IsPermanent || s.EndAt > now))
                .OrderByDescending(s => s.CreatedAt)
                .ToList();

            LoadSanctionsForPage(_currentPage, activeSanctions);
            SelectedPageIndex = _currentPage + 1;
            CanLoadPreviousPage = _currentPage > 0;
            CanLoadNextPage = _totalSanctions > (_currentPage + 1) * PageSize;
        }

        private void GoToPage()
        {
            int backendPageIndex = SelectedPageIndex - 1;
            int maxPageIndex = Math.Max(0, (int)Math.Ceiling((double)_totalSanctions / PageSize) - 1);

            if (backendPageIndex < 0 || backendPageIndex > maxPageIndex)
            {
                NotificationService.Notify($"Index de page invalide. Veuillez saisir un index entre 1 et {maxPageIndex + 1}.", NotificationType.Warning);
                SelectedPageIndex = _currentPage + 1;
                return;
            }

            _currentPage = backendPageIndex;

            // Filtrer pour n'afficher que les sanctions actives
            var now = DateTime.Now;
            var activeSanctions = _sanctionManager.ListOfSanctions
                .Where(s => s.Status == "active" && (s.IsPermanent || s.EndAt > now))
                .OrderByDescending(s => s.CreatedAt)
                .ToList();

            LoadSanctionsForPage(_currentPage, activeSanctions);
        }

        private void LoadSanctionsForPage(int pageIndex, List<SanctionsUser> activeSanctions)
        {
            var sanctions = GetSanctions(pageIndex * PageSize, PageSize, activeSanctions);

            Sanctions.Clear();
            foreach (var sanction in sanctions)
            {
                // Charger les entités liées si nécessaire
                if (sanction.User == null)
                {
                    sanction.User = _userManager.GetUserById(sanction.UserId);
                }

                if (sanction.TypeReportUsers == null && sanction.TypeReportUsersId > 0)
                {
                    sanction.TypeReportUsers = _typeReportManager.GetTypeReportById(sanction.TypeReportUsersId);
                }

                Sanctions.Add(sanction);
            }

            CanLoadPreviousPage = _currentPage > 0;
            CanLoadNextPage = _totalSanctions > (_currentPage + 1) * PageSize;
        }
    }
}