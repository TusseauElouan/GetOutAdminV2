using GetOutAdminV2.Managers;
using GetOutAdminV2.Services;
using GetOutAdminV2.ViewModels;
using GetOutAdminV2.Views;
using System.Diagnostics;
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
        public MainWindow()
        {
            InitializeComponent();
            var navigation = new NavigationViewModel();
            navigation.CurrentPage = new LogInPage();
        }
    }
}