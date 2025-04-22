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
        public IEnumerable<EReportStatus> AvailableStatuses => System.Enum.GetValues(typeof(EReportStatus))
                                                                        .Cast<EReportStatus>()
                                                                        .Where(status => status != EReportStatus.resolved)
                                                                        .ToList();

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
            Reports = new ObservableCollection<ReportUser>();

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

            _totalReports = CountReportsWithStatus(SelectedStatus);
            TotalPages = (int)Math.Ceiling((double)_totalReports / PageSize);

            // Initialiser avec la première page
            _currentPage = 0;
            LoadReportsForPage(_currentPage);
            SelectedPageIndex = _currentPage + 1;
            CanLoadPreviousPage = _currentPage > 0;
            CanLoadNextPage = _totalReports > (_currentPage + 1) * PageSize;
        }

        // Méthode pour compter les rapports avec un statut spécifique
        private int CountReportsWithStatus(EReportStatus status)
        {
            return _reportManager.ListOfReports.Count(r => r.Status == status.ToString());
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
                NotificationService.Notify("Ce report a été rejeté. Impossible d'appliquer une sanction.",
                    NotificationType.Warning, 5);
                return;
            }

            IsPopupNotOpen = false;

            // Vérifier si l'utilisateur signalé a déjà une sanction active
            var reportedUserId = SelectedReport.ReportedUserId;
            var activeSanction = _sanctionManager.GetActiveSanctionByUserId(reportedUserId);

            if (activeSanction != null)
            {
                string endDate = activeSanction.IsPermanent ? "permanente" : $"jusqu'au {activeSanction.EndAt?.ToString("dd/MM/yyyy")}";
                NotificationService.Notify($"Cet utilisateur a déjà une sanction active {endDate}.", NotificationType.Warning,10);
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

                NotificationService.Notify($"L'utilisateur {SelectedReport.ReportedUser.Nom} a été sanctionné avec succès.", NotificationType.Success);

                IsSanctionPopupOpen = false;

                // Rafraîchir les rapports
                LoadReports();
            }
            catch (Exception ex)
            {
                NotificationService.Notify($"Erreur lors de l'application de la sanction : {ex.Message}", NotificationType.Error);
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

                NotificationService.Notify($"Le statut du report a été mis à jour avec succès.", NotificationType.Success);

                IsChangeStatusPopupOpen = false;

                // Forcer une actualisation complète des reports
                _reportManager.GetAllReports();

                // Rafraîchir la liste filtrée selon le statut sélectionné
                LoadReports();

                // Si le statut a changé et ne correspond plus au filtre actuel, l'élément ne sera plus visible
                if (NewReportStatus != SelectedStatus)
                {
                    SelectedReport = null; // Désélectionner le rapport qui n'est plus visible
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify($"Erreur lors de la mise à jour du statut : {ex.Message}", NotificationType.Error);
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
            // Recharger les rapports quand le statut change
            LoadReports();

            // Recalculer le nombre total de rapports et les pages
            _totalReports = CountReportsWithStatus(value);
            TotalPages = (int)Math.Ceiling((double)_totalReports / PageSize);

            // Réinitialiser la pagination
            _currentPage = 0;
            SelectedPageIndex = 1;
            CanLoadPreviousPage = false;
            CanLoadNextPage = _totalReports > PageSize;

            // Charger la première page
            LoadReportsForPage(_currentPage);
        }

        private void LoadReports()
        {
            try
            {
                LoadingVisibility = nameof(EVisibility.Visible);
                DataGridVisibility = nameof(EVisibility.Hidden);

                // Filtrer les rapports selon le statut sélectionné
                var filteredReports = _reportManager.ListOfReports
                                         .Where(report => report.Status == SelectedStatus.ToString())
                                         .ToList();

                Reports.Clear();
                foreach (var report in filteredReports)
                {
                    // S'assurer que les références sont initialisées
                    if (report.ReportedUser == null)
                    {
                        var reportedUser = _userManager.GetUserById(report.ReportedUserId);
                        report.ReportedUser = reportedUser ?? new User { Nom = "Utilisateur inconnu" };
                    }

                    if (report.Reporter == null)
                    {
                        var reporter = _userManager.GetUserById(report.ReporterId);
                        report.Reporter = reporter ?? new User { Nom = "Utilisateur inconnu" };
                    }

                    if (report.TypeReport == null)
                    {
                        var typeReport = _typeReportManager.GetTypeReportById(report.TypeReportId);
                        report.TypeReport = typeReport ?? new TypeReportUser { Name = "Type inconnu" };
                    }

                    Reports.Add(report);
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify($"Erreur lors du chargement des reports : {ex.Message}", NotificationType.Error);
            }
            finally
            {
                LoadingVisibility = nameof(EVisibility.Hidden);
                DataGridVisibility = nameof(EVisibility.Visible);
            }
        }

        private IEnumerable<ReportUser> GetReports(int startIndex, int count)
        {
            // Filtrer les reports par le statut sélectionné
            return _reportManager.ListOfReports
                  .Where(r => r.Status == SelectedStatus.ToString())
                  .Skip(startIndex)
                  .Take(count);
        }

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
                NotificationService.Notify($"Index de page invalide. Veuillez saisir un index entre 1 et {maxPageIndex + 1}.", NotificationType.Warning);
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
                // Assurer que les références sont chargées
                if (report.ReportedUser == null)
                {
                    var reportedUser = _userManager.GetUserById(report.ReportedUserId);
                    report.ReportedUser = reportedUser ?? new User { Nom = "Utilisateur inconnu" };
                }

                if (report.Reporter == null)
                {
                    var reporter = _userManager.GetUserById(report.ReporterId);
                    report.Reporter = reporter ?? new User { Nom = "Utilisateur inconnu" };
                }

                if (report.TypeReport == null)
                {
                    var typeReport = _typeReportManager.GetTypeReportById(report.TypeReportId);
                    report.TypeReport = typeReport ?? new TypeReportUser { Name = "Type inconnu" };
                }

                Reports.Add(report);
            }

            CanLoadPreviousPage = _currentPage > 0;
            CanLoadNextPage = _totalReports > (_currentPage + 1) * PageSize;
        }
    }
}