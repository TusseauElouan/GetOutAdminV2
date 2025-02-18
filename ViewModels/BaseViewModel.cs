using CommunityToolkit.Mvvm.ComponentModel;
using GetOutAdminV2.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetOutAdminV2.ViewModels
{
    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool? _isLoading = false;

        [ObservableProperty]
        private string? _email;

        [ObservableProperty]
        private string? _password;

        [ObservableProperty]
        private object _currentPage;

        public void NavigateTo(object newPage)
        {
            CurrentPage = newPage;
            OnPropertyChanged(nameof(CurrentPage));
        }
    }
}
