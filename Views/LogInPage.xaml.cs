using GetOutAdminV2.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace GetOutAdminV2.Views
{
    /// <summary>
    /// Logique d'interaction pour LogInPage.xaml
    /// </summary>
    public partial class LogInPage : Page
    {
        private LogInViewModel? _viewModel;

        public LogInPage()
        {
            InitializeComponent();
            this.Loaded += LogInPage_Loaded;
        }

        private void LogInPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow?.DataContext is NavigationViewModel navViewModel)
            {
                _viewModel = new LogInViewModel(navViewModel);
                this.DataContext = _viewModel;
            }
        }

        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null && sender is PasswordBox passwordBox)
            {
                _viewModel.Password = passwordBox.Password;
            }
        }
    }
}
