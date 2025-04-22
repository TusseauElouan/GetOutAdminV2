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
using System.Windows;
using System.Windows.Media;

namespace GetOutAdminV2.ViewModels
{
    public partial class ReportViewModel : BaseViewModel
    {
        private const int PageSize = 20; // Nombre de rapports par page
        private int _currentPage = -1; // Page actuelle (index backend)
        private int _totalReports; // Nombre total de rapports

        private readonly IReportManager _reportManager;
        private readonly IUserManager _userManager;
        private readonly ITypeReportManager _typeReportManager;
        private readonly ISanctionManager _sanctionManager;

        public ObservableCollection<ReportUser> Reports { get; }

        [ObservableProperty]
        private ReportUser? selectedReport;

        [ObservableProperty]
        private bool isSanctionPopupOpen;

        [ObservableProperty]
        private bool isChangeStatusPopupOpen;

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
        private EReportStatus _selectedStatus = EReportStatus.pending;

        [ObservableProperty]
        private EReportStatus _newReportStatus = EReportStatus.investigating;

        [ObservableProperty]
        private ESanctionDuration _selectedSanctionDuration = ESanctionDuration.TwoWeeks;

        [ObservableProperty]
        private string _sanctionDescription = string.Empty;

        [ObservableProperty]
        private string _statusChangedNote = string.Empty;

        public IEnumerable<ESanctionDuration> SanctionDurations => System.Enum.GetValues(typeof(ESanctionDuration)).Cast<ESanctionDuration>();
        public IEnumerable<EReportStatus> ReportStatuses => System.Enum.GetValues(typeof(EReportStatus)).Cast<EReportStatus>();

        public IRelayCommand LoadNextPageCommand { get; }
        public IRelayCommand LoadPreviousPageCommand { get; }
        public IRelayCommand GoToPageCommand { get; }
        public IRelayCommand ShowSanctionPopupCommand { get; }
        public IRelayCommand ConfirmSanctionCommand { get; }
        public IRelayCommand CancelSanctionCommand { get; }
        public IRelayCommand ShowChangeStatusPopupCommand { get; }
        public IRelayCommand ConfirmChangeStatusCommand { get; }
        public IRelayCommand CancelChangeStatusCommand { get; }

        // Propriété pour vérifier si un rapport est sélectionné et peut être sanctionné
        public bool CanSanctionReport => SelectedReport != null && SelectedReport.Status != EReportStatus.rejected.ToString();
        public bool CanChangeStatus => SelectedReport != null;

        public ReportViewModel()
        {
            _reportManager = ServiceLocator.GetRequiredService<IReportManager>();
            _userManager = ServiceLocator.GetRequiredService<IUserManager>();
            _typeReportManager = ServiceLocator.GetRequiredService<ITypeReportManager>();
            _sanctionManager = ServiceLocator.GetRequiredService<ISanctionManager>();
            SelectedStatus = EReportStatus.pending;
            _reportManager.GetAllReports();
            _typeReportManager.GetAllTypeReports();
            Reports = new(_reportManager.ListOfReports);

            LoadReports(); // Charge initialement les rapports avec les statuts filtrés
            LoadNextPageCommand = new RelayCommand(LoadNextPage);
            LoadPreviousPageCommand = new RelayCommand(LoadPreviousPage);
            GoToPageCommand = new RelayCommand(GoToPage);

            // Commandes pour la gestion des sanctions
            ShowSanctionPopupCommand = new RelayCommand(ShowSanctionPopup, () => CanSanctionReport);
            ConfirmSanctionCommand = new RelayCommand(ConfirmSanction);
            CancelSanctionCommand = new RelayCommand(CancelSanction);

            // Commandes pour le changement de statut
            ShowChangeStatusPopupCommand = new RelayCommand(ShowChangeStatusPopup, () => SelectedReport != null);
            ConfirmChangeStatusCommand = new RelayCommand(ConfirmChangeStatus);
            CancelChangeStatusCommand = new RelayCommand(CancelChangeStatus);

            _totalReports = _reportManager.ListOfReports.Count;
            TotalPages = (int)Math.Ceiling((double)_totalReports / PageSize);
            LoadReportsForPage(_currentPage);
        }

        // Méthode exécutée lorsque SelectedReport change
        partial void OnSelectedReportChanged(ReportUser? oldValue, ReportUser? newValue)
        {
            // Notifier les commandes de vérifier à nouveau leurs conditions d'activation
            (ShowSanctionPopupCommand as RelayCommand)?.NotifyCanExecuteChanged();
            (ShowChangeStatusPopupCommand as RelayCommand)?.NotifyCanExecuteChanged();
        }

        // Méthode pour vérifier si un rapport peut être sanctionné (ne doit pas être déjà rejeté)
        private bool CanSanctionSelectedReport()
        {
            return SelectedReport != null && SelectedReport.Status != EReportStatus.rejected.ToString();
        }

        private void ShowSanctionPopup()
        {
            if (IsSanctionPopupOpen || SelectedReport == null) return;

            // Vérifier si le report est déjà rejeté, auquel cas on ne peut pas sanctionner
            if (SelectedReport.Status == EReportStatus.rejected.ToString())
            {
                MessageBox.Show("Ce report a été rejeté. Impossible d'appliquer une sanction.",
                    "Action impossible", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            IsPopupNotOpen = false;

            // Vérifier si l'utilisateur signalé a déjà une sanction active
            var reportedUserId = SelectedReport.ReportedUserId;
            var activeSanction = _sanctionManager.GetActiveSanctionByUserId(reportedUserId);

            if (activeSanction != null)
            {
                string endDate = activeSanction.IsPermanent ? "permanente" : $"jusqu'au {activeSanction.EndAt?.ToString("dd/MM/yyyy")}";
                MessageBox.Show($"Cet utilisateur a déjà une sanction active {endDate}.", "Sanction existante", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Réinitialiser les valeurs
            SelectedSanctionDuration = ESanctionDuration.TwoWeeks;
            SanctionDescription = SelectedReport.Description ?? string.Empty;

            IsSanctionPopupOpen = true;
        }

        private void ShowChangeStatusPopup()
        {
            if (IsChangeStatusPopupOpen || SelectedReport == null) return;
            IsPopupNotOpen = false;

            // Définir le statut par défaut à afficher dans la popup
            try
            {
                NewReportStatus = System.Enum.Parse<EReportStatus>(SelectedReport.Status);
            }
            catch
            {
                NewReportStatus = EReportStatus.investigating;
            }

            StatusChangedNote = string.Empty;
            IsChangeStatusPopupOpen = true;
        }

        private void ConfirmSanction()
        {
            if (SelectedReport == null) return;

            try
            {
                LoadingVisibility = nameof(EVisibility.Visible);
                DataGridVisibility = nameof(EVisibility.Hidden);

                // Créer la nouvelle sanction
                var now = DateTime.Now;

                var sanction = new SanctionsUser
                {
                    UserId = SelectedReport.ReportedUserId,
                    TypeReportUsersId = SelectedReport.TypeReportId,
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

                // Mettre à jour le statut du rapport à "resolved"
                SelectedReport.Status = EReportStatus.resolved.ToString();
                SelectedReport.ResolvedAt = now;
                SelectedReport.ResolvedBy = _userManager.CurrentUser?.Id;
                SelectedReport.ResolutionNote = $"Utilisateur sanctionné - {SelectedSanctionDuration.GetDisplayName()}";
                _reportManager.UpdateReport(SelectedReport);

                MessageBox.Show($"L'utilisateur {SelectedReport.ReportedUser.Nom} a été sanctionné avec succès.", "Sanction appliquée", MessageBoxButton.OK, MessageBoxImage.Information);

                IsSanctionPopupOpen = false;

                // Rafraîchir les rapports
                LoadReports();
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

        private void ConfirmChangeStatus()
        {
            if (SelectedReport == null) return;

            try
            {
                LoadingVisibility = nameof(EVisibility.Visible);
                DataGridVisibility = nameof(EVisibility.Hidden);

                var now = DateTime.Now;

                // Mettre à jour le statut du rapport
                SelectedReport.Status = NewReportStatus.ToString();

                // Si le statut est resolved ou rejected, mettre à jour les champs correspondants
                if (NewReportStatus == EReportStatus.resolved || NewReportStatus == EReportStatus.rejected)
                {
                    SelectedReport.ResolvedAt = now;
                    SelectedReport.ResolvedBy = _userManager.CurrentUser?.Id;
                    SelectedReport.ResolutionNote = StatusChangedNote;
                }

                _reportManager.UpdateReport(SelectedReport);

                MessageBox.Show($"Le statut du report a été mis à jour avec succès.", "Statut mis à jour", MessageBoxButton.OK, MessageBoxImage.Information);

                IsChangeStatusPopupOpen = false;

                // Forcer une actualisation complète des reports
                _reportManager.GetAllReports();

                // Rafraîchir la liste filtrée selon le statut sélectionné
                LoadReports();

                // Assurer que la liste affichée correspond au status actuel
                SelectedStatus = NewReportStatus;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la mise à jour du statut : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void CancelChangeStatus()
        {
            IsChangeStatusPopupOpen = false;
        }

        partial void OnSelectedStatusChanged(EReportStatus value)
        {
            LoadReports(); // Recharger les rapports quand le statut change
        }

        private void LoadReports()
        {
            // Filtrer les rapports selon le statut sélectionné
            var filteredReports = _reportManager.ListOfReports
                                     .Where(report => report.Status == SelectedStatus.ToString())
                                     .ToList();

            Reports.Clear();
            foreach (var report in filteredReports)
            {
                // Remplacer les IDs par les noms
                report.ReportedUser = new User() { Nom = _userManager.GetUserById(report.ReportedUserId)?.Nom ?? "Utilisateur inconnu" };
                report.Reporter = new User() { Nom = _userManager.GetUserById(report.ReporterId)?.Nom ?? "Utilisateur inconnu" };
                report.TypeReport = new TypeReportUser() { Name = _typeReportManager.GetTypeReportById(report.TypeReportId).Name ?? "Type de signalement inconnu" };

                Reports.Add(report);
            }
        }

        private IEnumerable<ReportUser> GetReports(int startIndex, int count) => _reportManager.ListOfReports.Skip(startIndex).Take(count);

        private void LoadNextPage()
        {
            _currentPage++;
            LoadReportsForPage(_currentPage);
            SelectedPageIndex = _currentPage + 1;
            CanLoadPreviousPage = _currentPage > 0;
            CanLoadNextPage = _totalReports > (_currentPage + 1) * PageSize;
        }

        private void LoadPreviousPage()
        {
            _currentPage--;
            LoadReportsForPage(_currentPage);
            SelectedPageIndex = _currentPage + 1;
            CanLoadPreviousPage = _currentPage > 0;
            CanLoadNextPage = _totalReports > (_currentPage + 1) * PageSize;
        }

        private void GoToPage()
        {
            int backendPageIndex = SelectedPageIndex - 1;
            int maxPageIndex = (int)Math.Ceiling((double)_totalReports / PageSize) - 1;

            if (backendPageIndex < 0 || backendPageIndex > maxPageIndex)
            {
                MessageBox.Show($"Index de page invalide. Veuillez saisir un index entre 1 et {maxPageIndex + 1}.");
                SelectedPageIndex = _currentPage + 1;
                return;
            }

            _currentPage = backendPageIndex;
            LoadReportsForPage(_currentPage);
        }

        private void LoadReportsForPage(int pageIndex)
        {
            var reports = GetReports(pageIndex * PageSize, PageSize);

            Reports.Clear();
            foreach (var report in reports)
            {
                Reports.Add(report);
            }

            CanLoadPreviousPage = _currentPage > 0;
            CanLoadNextPage = _totalReports > (_currentPage + 1) * PageSize;
        }
    }
}