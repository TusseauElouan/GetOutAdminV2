using CommunityToolkit.Mvvm.Input;
using GetOutAdminV2.Managers;
using GetOutAdminV2.Models;
using GetOutAdminV2.Services;

namespace GetOutAdminV2.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    public partial class ListUsersViewModel : BaseViewModel
    {
        private const int PageSize = 1; // Nombre d'utilisateurs par page
        private int _currentPage = -1; // Page actuelle (index backend)
        private int _totalUsers; // Nombre total d'utilisateurs
        public ObservableCollection<User> Users { get; set; }

        [ObservableProperty]
        private string? _pageInfo;

        [ObservableProperty]
        private int _selectedPageIndex = 1; // Index visuel (commence à 1)

        [ObservableProperty]
        private int _totalPages;

        public ICommand LoadNextPageCommand { get; }
        public ICommand LoadPreviousPageCommand { get; }
        public ICommand GoToPageCommand { get; }

        private readonly IUserManager _userManager;

        public ListUsersViewModel()
        {
            Users = new ObservableCollection<User>();
            LoadNextPageCommand = new RelayCommand(LoadNextPage);
            LoadPreviousPageCommand = new RelayCommand(LoadPreviousPage);
            GoToPageCommand = new RelayCommand(GoToPage);

            // Initialise le nombre total d'utilisateurs
            _userManager = ServiceLocator.GetRequiredService<IUserManager>();
            _totalUsers = ListOfUsers.Count;
            TotalPages = (int)Math.Ceiling((double)_totalUsers / PageSize);
            LoadNextPage(); // Charge la première page au démarrage
        }

        public ObservableCollection<User> ListOfUsers => _userManager.ListOfUsers;

        [ObservableProperty]
        private bool _canLoadPreviousPage;
        [ObservableProperty]
        private bool _canLoadNextPage;

        private IEnumerable<User> GetUsers(int startIndex, int count) => ListOfUsers.Skip(startIndex).Take(count);

        private void LoadNextPage()
        {
            // Charge la page suivante
            _currentPage++;
            LoadUsersForPage(_currentPage);
            SelectedPageIndex = _currentPage + 1; // Met à jour l'index visuel
            CanLoadPreviousPage = _currentPage > 0;
            CanLoadNextPage = _totalUsers > (_currentPage + 1) * PageSize;
        }

        private void LoadPreviousPage()
        {
            // Charge la page précédente
            _currentPage--;
            LoadUsersForPage(_currentPage);
            SelectedPageIndex = _currentPage + 1; // Met à jour l'index visuel
            CanLoadPreviousPage = _currentPage > 0;
            CanLoadNextPage = _totalUsers > (_currentPage + 1) * PageSize;
        }

        private void GoToPage()
        {
            // Convertit l'index visuel en index backend
            int backendPageIndex = SelectedPageIndex - 1;

            // Vérifie que l'index de page est valide
            int maxPageIndex = (int)Math.Ceiling((double)_totalUsers / PageSize) - 1;
            if (backendPageIndex < 0 || backendPageIndex > maxPageIndex)
            {
                MessageBox.Show($"Index de page invalide. Veuillez saisir un index entre 1 et {maxPageIndex + 1}.");
                SelectedPageIndex = _currentPage + 1; // Réinitialise l'index visuel
                return;
            }

            // Charge les utilisateurs de la page spécifiée
            _currentPage = backendPageIndex;
            LoadUsersForPage(_currentPage);
        }

        private void LoadUsersForPage(int pageIndex)
        {
            // Récupère les utilisateurs pour la page spécifiée
            var users = GetUsers(pageIndex * PageSize, PageSize);

            // Met à jour la liste des utilisateurs
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