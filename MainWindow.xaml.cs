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

        public Frame NavigationFrame => this.ContentFrame;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LogInViewModel viewModel)
            {
                var passwordBox = (PasswordBox)sender;
                viewModel.Password = passwordBox.Password;
            }
        }
    }
}