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
    using System.Windows.Input;

    public partial class ListUsersViewModel : BaseViewModel
    {
        private const int PageSize = 50; // Nombre d'utilisateurs par page
        private int _currentPage = 0; // Page actuelle
        private int _totalUsers; // Nombre total d'utilisateurs
        public ObservableCollection<User> Users { get; set; }

        [ObservableProperty]
        private string? _pageInfo;
        public ICommand LoadNextPageCommand { get; }
        public ICommand LoadPreviousPageCommand { get; }

        private readonly IUserManager _userManager;
        public ListUsersViewModel()
        {
            Users = new ObservableCollection<User>();
            LoadNextPageCommand = new RelayCommand(LoadNextPage, CanLoadNextPage);
            LoadPreviousPageCommand = new RelayCommand(LoadPreviousPage, CanLoadPreviousPage);

            // Initialise le nombre total d'utilisateurs
            _userManager = ServiceLocator.GetRequiredService<IUserManager>();
            _totalUsers = ListOfUsers.Count;
            LoadNextPage(); // Charge la première page au démarrage
        }
        public ObservableCollection<User> ListOfUsers => _userManager.ListOfUsers;

        private void LoadNextPage()
        {
            // Récupère les utilisateurs pour la page actuelle
            var nextUsers = GetUsers(_currentPage * PageSize, PageSize);

            // Ajoute les utilisateurs à la liste
            Users.Clear();
            foreach (var user in nextUsers)
            {
                Users.Add(user);
            }

            // Passe à la page suivante
            _currentPage++;

            // Met à jour la possibilité de charger la page précédente
            CanLoadPreviousPage();

            // Met à jour l'affichage de la pagination
            UpdatePageInfo();
        }

        private void LoadPreviousPage()
        {
            // Recharge les utilisateurs de la page précédente
            _currentPage--;
            var previousUsers = GetUsers(_currentPage * PageSize, PageSize);

            Users.Clear();
            foreach (var user in previousUsers)
            {
                Users.Add(user);
            }

            // Met à jour la possibilité de charger la page précédente
            CanLoadPreviousPage();

            // Met à jour l'affichage de la pagination
            UpdatePageInfo();
        }

        private bool CanLoadNextPage()
        {
            // Vérifie s'il reste des utilisateurs à charger
            return _totalUsers > (_currentPage + 1) * PageSize;
        }

        private bool CanLoadPreviousPage()
        {
            // Vérifie si on peut revenir à la page précédente
            return _currentPage > 0;
        }

        private IEnumerable<User> GetUsers(int startIndex, int count)
        {
            // Simule la récupération des utilisateurs depuis une source de données
            return ListOfUsers.Skip(startIndex).Take(count);
        }

        private void UpdatePageInfo()
        {
            // Calcule l'index de début et de fin de la page actuelle
            int startIndex = _currentPage * PageSize + 1;
            int endIndex = Math.Min((_currentPage + 1) * PageSize, _totalUsers);

            // Met à jour l'affichage de la pagination
            PageInfo = $"{startIndex}-{endIndex} sur {_totalUsers}";
        }
    }
}