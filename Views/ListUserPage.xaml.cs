﻿using GetOutAdminV2.Managers;
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
    /// Logique d'interaction pour ListUserPage.xaml
    /// </summary>
    public partial class ListUserPage : Page
    {
        private ListUsersViewModel? _viewModel;
        public ListUserPage()
        {
            InitializeComponent();
            this.DataContext = new ListUsersViewModel();
        }
    }
}
