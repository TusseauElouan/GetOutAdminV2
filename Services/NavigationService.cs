using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetOutAdminV2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GetOutAdminV2.Services
{
    public partial class NavigationService : BaseViewModel
    {
        [ObservableProperty]
        private object _currentPage;

        public ICommand ListUsersCommand { get; set; }

        private void ListUsers() => CurrentPage = new ListUsersViewModel();

        public NavigationService()
        {
            ListUsersCommand = new RelayCommand(ListUsers);
        }
    }
}
