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
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

        public ObservableCollection<ReportUser> Reports { get; }

        [ObservableProperty]
        private ReportUser? selectedReport;

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

        [ObservableProperty]
        private EReportStatus _selectedStatus;

        public IRelayCommand LoadNextPageCommand { get; }
        public IRelayCommand LoadPreviousPageCommand { get; }
        public IRelayCommand GoToPageCommand { get; }
        public IRelayCommand ShowDeletePopupCommand { get; }
        public IRelayCommand ConfirmDeleteCommand { get; }
        public IRelayCommand CancelDeleteCommand { get; }

        // Propriété pour vérifier si un rapport est sélectionné
        public bool CanDeleteReport => SelectedReport != null;

        public ReportViewModel()
        {
            _reportManager = ServiceLocator.GetRequiredService<IReportManager>();
            _userManager = ServiceLocator.GetRequiredService<IUserManager>();
            _typeReportManager = ServiceLocator.GetRequiredService<ITypeReportManager>();
            _reportManager.GetAllReports();
            Reports = new (_reportManager.ListOfReports);

            LoadReports(); // Charge initialement les rapports avec les statuts filtrés
            LoadNextPageCommand = new RelayCommand(LoadNextPage);
            LoadPreviousPageCommand = new RelayCommand(LoadPreviousPage);
            GoToPageCommand = new RelayCommand(GoToPage);

            _totalReports = _reportManager.ListOfReports.Count;
            TotalPages = (int)Math.Ceiling((double)_totalReports / PageSize);
            LoadReportsForPage(_currentPage);
            var test = Reports;
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
