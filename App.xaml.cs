using GetOutAdminV2.Managers;
using GetOutAdminV2.Models;
using GetOutAdminV2.Providers;
using GetOutAdminV2.Services;
using GetOutAdminV2.ViewModels;
using GetOutAdminV2.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

namespace GetOutAdminV2._0
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    var connectionString = context.Configuration.GetConnectionString("DefaultConnection");
                    services.AddDbContextFactory<AppDbContext>(options =>
                    {
                        options.UseNpgsql(connectionString);
#if DEBUG
                        options.EnableSensitiveDataLogging();
                        options.EnableDetailedErrors();
#endif
                    });

                    // Enregistrement des services
                    services.AddSingleton<IUserProvider, UserProvider>();
                    services.AddSingleton<IUserManager, UserManager>();
                    //services.AddSingleton<LogInViewModel>();
                    services.AddSingleton<MainWindow>();
                    services.AddSingleton<NavigationViewModel>();
                })
                .Build();

            // Initialiser le ServiceLocator
            ServiceLocator.Initialize(_host.Services);
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();
            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await _host.StopAsync();
            _host.Dispose();
            base.OnExit(e);
        }
    }

}
