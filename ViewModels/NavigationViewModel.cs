using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetOutAdminV2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GetOutAdminV2.ViewModels
{
    public partial class NavigationViewModel : BaseViewModel
    {
        [ObservableProperty]
        private object _currentPage;

        public ICommand ListUsersCommand { get; set; }
        public ICommand LogInCommand { get; set; }
        public ICommand DashBoardCommand { get; set; }

        private void ListUsers() => CurrentPage = new ListUsersViewModel();

        private void LogIn() => CurrentPage = new LogInViewModel();

        private void DashBoard() => CurrentPage = new DashBoardViewModel();

        public NavigationViewModel()
        {
            LogInCommand = new RelayCommand(LogIn);
            ListUsersCommand = new RelayCommand(ListUsers);
            DashBoardCommand = new RelayCommand(DashBoard);
        }
    }
}
