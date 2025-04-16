using GetOutAdminV2.Services;
using System.Windows;
using System.Windows.Controls;

namespace GetOutAdminV2.Controls
{
    /// <summary>
    /// Logique d'interaction pour NotificationHost.xaml
    /// </summary>
    public partial class NotificationHost : UserControl
    {
        public NotificationHost()
        {
            InitializeComponent();
            NotificationService.OnNotificationRequested += ShowNotification;
        }

        private void ShowNotification(string message, NotificationType type, int durationInSeconds)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var notification = NotificationService.CreateNotification(message, type, durationInSeconds);

                // Ajouter la notification au conteneur
                if (NotificationsPanel is ItemsControl itemsControl)
                {
                    itemsControl.Items.Add(notification);
                }
            });
        }

        ~NotificationHost()
        {
            // Désabonner l'événement pour éviter les fuites de mémoire
            NotificationService.OnNotificationRequested -= ShowNotification;
        }
    }
}