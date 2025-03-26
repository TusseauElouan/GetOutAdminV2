using CommunityToolkit.Mvvm.ComponentModel;
using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetOutAdminV2.Managers;
using GetOutAdminV2.Services;
using GetOutAdminV2.Models;
using System.Collections.ObjectModel;
using System.Globalization;

namespace GetOutAdminV2.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly IUserManager _userManager;

        [ObservableProperty]
        private int totalUsers;

        [ObservableProperty]
        private SeriesCollection userGrowthChart;

        [ObservableProperty]
        private ObservableCollection<User> recentUsers = new();

        [ObservableProperty]
        private string[] labels;

        public DashboardViewModel()
        {
            _userManager = ServiceLocator.GetRequiredService<IUserManager>();
            _userManager.UsersUpdated += LoadData;
            LoadData();
        }

        private void LoadData()
        {
            // Nombre total d'utilisateurs
            TotalUsers = _userManager.ListOfUsers.Count;

            // Liste des 5 derniers utilisateurs ajoutés
            RecentUsers = new ObservableCollection<User>(
                _userManager.ListOfUsers.OrderByDescending(u => u.CreatedAt).Take(5)
            );

            // Graphique d'évolution des utilisateurs
            var userGrowth = _userManager.ListOfUsers
                .Where(u => u.CreatedAt.HasValue)
                .GroupBy(u => new { u.CreatedAt.Value.Year, u.CreatedAt.Value.Month })
                .OrderBy(g => g.Key.Year)
                .ThenBy(g => g.Key.Month)
                .ToDictionary(
                    g => new DateTime(g.Key.Year, g.Key.Month, 1),
                    g => g.Count()
                );

            // Créer un nouveau dictionnaire avec des valeurs cumulatives
            var cumulativeUserGrowth = new Dictionary<DateTime, int>();
            int cumulativeTotal = 0;
            foreach (var item in userGrowth.OrderBy(x => x.Key))
            {
                cumulativeTotal += item.Value;
                cumulativeUserGrowth[item.Key] = cumulativeTotal;
            }

            UserGrowthChart = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Utilisateurs",
                    Values = new ChartValues<int>(cumulativeUserGrowth.Values)
                }
            };

            // Formater les labels pour afficher Mois Année
            Labels = cumulativeUserGrowth.Keys
                .Select(date => date.ToString("MMMM yyyy", new CultureInfo("fr-FR")))
                .ToArray();
        }
    }
}