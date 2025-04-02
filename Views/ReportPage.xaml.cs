using GetOutAdminV2.ViewModels;
using System.Windows.Controls;

namespace GetOutAdminV2.Views
{
    /// <summary>
    /// Logique d'interaction pour ReportPage.xaml
    /// </summary>
    public partial class ReportPage : Page
    {
        public ReportPage()
        {
            InitializeComponent();
            this.DataContext = new ReportViewModel();
        }
    }
}
