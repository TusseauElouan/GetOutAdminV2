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
        public ICommand DashBoardCommand { get; set; }
        public ICommand ReportCommand { get; set; }
        public ICommand SanctionsCommand { get; set; }

        [ObservableProperty]
        private bool _isNotLogInPage;

        private void ListUsers()
        {
            IsNotLogInPage = true;
            CurrentPage = new ListUserPage();
        }

        private void LogIn()
        {
            CurrentPage = new LogInPage();
            IsNotLogInPage = false;
        }

        private void DashBoard()
        {
            CurrentPage = new DashBoardPage();
            IsNotLogInPage = true;
        }

        private void Report()
        {
            CurrentPage = new ReportPage();
            IsNotLogInPage = true;
        }

        private void Sanctions()
        {
            CurrentPage = new SanctionsPage();
            IsNotLogInPage = true;
        }

        public NavigationViewModel()
        {
            ListUsersCommand = new RelayCommand(ListUsers);
            LogInCommand = new RelayCommand(LogIn);
            DashBoardCommand = new RelayCommand(DashBoard);
            ReportCommand = new RelayCommand(Report);
            SanctionsCommand = new RelayCommand(Sanctions);
            CurrentPage = new LogInPage();
            IsNotLogInPage = false;
        }
    }
}