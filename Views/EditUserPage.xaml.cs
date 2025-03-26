using GetOutAdminV2.ViewModels;
using System.Windows.Controls;

namespace GetOutAdminV2.Views
{
    public partial class EditUserPage : Page
    {
        public EditUserPage(long userId)
        {
            InitializeComponent();
            DataContext = new EditUserViewModel(userId); // ✅ Passe l'ID au ViewModel
        }
    }
}