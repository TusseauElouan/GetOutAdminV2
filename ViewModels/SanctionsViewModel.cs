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

        public IRelayCommand LoadNextPageCommand { get; }
        public IRelayCommand LoadPreviousPageCommand { get; }
        public IRelayCommand GoToPageCommand { get; }

        public SanctionsViewModel()
        {
            _sanctionManager = ServiceLocator.GetRequiredService<ISanctionManager>();
            _userManager = ServiceLocator.GetRequiredService<IUserManager>();
            _typeReportManager = ServiceLocator.GetRequiredService<ITypeReportManager>();

            Sanctions = new ObservableCollection<SanctionsUser>();

            LoadNextPageCommand = new RelayCommand(LoadNextPage);
            LoadPreviousPageCommand = new RelayCommand(LoadPreviousPage);
            GoToPageCommand = new RelayCommand(GoToPage);

            LoadSanctions();
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
                    MessageBox.Show("Aucune sanction active n'a été trouvée.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                LoadSanctionsForPage(_currentPage, activeSanctions);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des sanctions : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                LoadingVisibility = nameof(EVisibility.Hidden);
                DataGridVisibility = nameof(EVisibility.Visible);
            }
        }

        private IEnumerable<SanctionsUser> GetSanctions(int startIndex, int count, List<SanctionsUser> sanctions)
        {
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
            int maxPageIndex = (int)Math.Ceiling((double)_totalSanctions / PageSize) - 1;

            if (backendPageIndex < 0 || backendPageIndex > maxPageIndex)
            {
                MessageBox.Show($"Index de page invalide. Veuillez saisir un index entre 1 et {maxPageIndex + 1}.");
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