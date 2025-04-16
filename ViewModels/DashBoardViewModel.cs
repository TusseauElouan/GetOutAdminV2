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
using GetOutAdminV2.Enum;
using System.Windows.Media;

namespace GetOutAdminV2.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly IUserManager _userManager;
        private readonly IReportManager _reportManager;

        [ObservableProperty]
        private int totalUsers;

        [ObservableProperty]
        private SeriesCollection userGrowthChart;

        [ObservableProperty]
        private SeriesCollection reportStatusChart;

        [ObservableProperty]
        private ObservableCollection<User> recentUsers = new();

        [ObservableProperty]
        private string[] labels;

        [ObservableProperty]
        private int pendingCount;

        [ObservableProperty]
        private int investigatingCount;

        [ObservableProperty]
        private int resolvedCount;

        [ObservableProperty]
        private int rejectedCount;

        [ObservableProperty]
        private int totalReports;

        public DashboardViewModel()
        {
            _userManager = ServiceLocator.GetRequiredService<IUserManager>();
            _reportManager = ServiceLocator.GetRequiredService<IReportManager>();

            _userManager.UsersUpdated += LoadData;
            _reportManager.ReportsUpdated += OnReportsUpdated; // Utilisez la nouvelle méthode

            // Chargez les données initialement
            _reportManager.GetAllReports(); // Charger les données une seule fois au démarrage
            LoadData();
            UpdateReportData();
        }

        private void LoadData()
        {
            // Nombre total d'utilisateurs (exclus les admins)
            TotalUsers = _userManager.ListOfUsers.Count(u => !u.IsAdmin);

            // Liste des 5 derniers utilisateurs ajoutés (exclus les admins)
            RecentUsers = new ObservableCollection<User>(
                _userManager.ListOfUsers
                    .Where(u => !u.IsAdmin)
                    .OrderByDescending(u => u.CreatedAt)
                    .Take(5)
            );

            // Graphique d'évolution des utilisateurs (exclus les admins)
            var userGrowth = _userManager.ListOfUsers
                .Where(u => u.CreatedAt.HasValue && !u.IsAdmin)
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

        private void UpdateReportData()
        {
            // Ne pas appeler GetAllReports ici, cela crée une boucle récursive
            // _reportManager.GetAllReports(); <-- SUPPRIMER CETTE LIGNE

            // Utilisez directement les données déjà chargées
            var allReports = _reportManager.ListOfReports;

            // Calculer le nombre total de reports
            TotalReports = allReports.Count;

            // Calculer le nombre de reports par statut
            PendingCount = allReports.Count(r => r.Status == EReportStatus.pending.ToString());
            InvestigatingCount = allReports.Count(r => r.Status == EReportStatus.investigating.ToString());
            ResolvedCount = allReports.Count(r => r.Status == EReportStatus.resolved.ToString());
            RejectedCount = allReports.Count(r => r.Status == EReportStatus.rejected.ToString());

            // Créer le graphique camembert avec les données
            InitializeReportStatusChart();
        }
        private void OnReportsUpdated()
        {
            // N'appelez pas GetAllReports() ici, les données sont déjà mises à jour dans ReportManager
            UpdateReportData();
        }
        private void InitializeReportStatusChart()
        {
            ReportStatusChart = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "En attente",
                    Values = new ChartValues<int> { PendingCount },
                    DataLabels = true,
                    LabelPoint = point => $"En attente: {point.Y} ({point.Participation:P1})",
                    Fill = new SolidColorBrush(Color.FromRgb(255, 165, 0)) // Orange
                },
                new PieSeries
                {
                    Title = "En cours d'investigation",
                    Values = new ChartValues<int> { InvestigatingCount },
                    DataLabels = true,
                    LabelPoint = point => $"Investigation: {point.Y} ({point.Participation:P1})",
                    Fill = new SolidColorBrush(Color.FromRgb(0, 119, 204)) // Bleu
                },
                new PieSeries
                {
                    Title = "Résolu",
                    Values = new ChartValues<int> { ResolvedCount },
                    DataLabels = true,
                    LabelPoint = point => $"Résolu: {point.Y} ({point.Participation:P1})",
                    Fill = new SolidColorBrush(Color.FromRgb(46, 204, 113)) // Vert
                },
                new PieSeries
                {
                    Title = "Rejeté",
                    Values = new ChartValues<int> { RejectedCount },
                    DataLabels = true,
                    LabelPoint = point => $"Rejeté: {point.Y} ({point.Participation:P1})",
                    Fill = new SolidColorBrush(Color.FromRgb(231, 76, 60)) // Rouge
                }
            };
        }
    }
}