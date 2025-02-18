using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetOutAdminV2.ViewModels;
using GetOutAdminV2.Views;
using System.Windows.Input;

namespace GetOutAdminV2.ViewModels
{
    public partial class NavigationViewModel : BaseViewModel
    {

        public ICommand ListUsersCommand { get; set; }
        public ICommand LogInCommand { get; set; }

        private void ListUsers() => CurrentPage = new ListUserPage();
        private void LogIn() => CurrentPage = new LogInPage(); // Ensure this is an instance of LogInPage

        public NavigationViewModel()
        {
            ListUsersCommand = new RelayCommand(ListUsers);
            LogInCommand = new RelayCommand(LogIn);
            CurrentPage = new LogInPage();
        }
    }
}
