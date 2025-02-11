using GetOutAdminV2.Managers;
using GetOutAdminV2.ViewModels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GetOutAdminV2._0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LogInViewModel vm = new LogInViewModel();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new LogInViewModel(new UserManager());
        }

        private void password_PasswordChanged(object sender, RoutedEventArgs e)
        {

            if (DataContext is LogInViewModel viewModel)
            {
                var passwordBox = (PasswordBox)sender;
                viewModel.Password = passwordBox.Password;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F11)
            {
                ToggleFullScreen();
            }
        }

        private void ToggleFullScreen()
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
                this.WindowStyle = WindowStyle.None;
            }
            else
            {
                this.WindowState = WindowState.Normal;
                this.WindowStyle = WindowStyle.SingleBorderWindow;
            }
        }
    }
}