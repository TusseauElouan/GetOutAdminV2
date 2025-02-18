using GetOutAdminV2.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GetOutAdminV2.Views
{
    /// <summary>
    /// Logique d'interaction pour LogInPage.xaml
    /// </summary>
    public partial class LogInPage : Page
    {
        public LogInPage()
        {
            InitializeComponent();
        }

        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LogInViewModel viewModel)
            {
                var passwordBox = (PasswordBox)sender;
                viewModel.Password = passwordBox.Password;
            }
        }
    }
}
