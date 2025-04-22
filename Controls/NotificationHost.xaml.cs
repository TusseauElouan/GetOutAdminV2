using GetOutAdminV2.Services;
using System;
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

            // S'assurer de se désabonner d'abord pour éviter les doublons
            NotificationService.OnNotificationRequested -= ShowNotification;
            NotificationService.OnNotificationRequested += ShowNotification;
        }

        private void ShowNotification(string message, NotificationType type, int durationInSeconds)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var notification = NotificationService.CreateNotification(message, type, durationInSeconds);

                    // Ajouter la notification au conteneur
                    if (NotificationsPanel != null && NotificationsPanel is ItemsControl itemsControl)
                    {
                        itemsControl.Items.Add(notification);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("NotificationsPanel est null ou n'est pas un ItemsControl");
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de l'affichage de la notification: {ex.Message}");
            }
        }

        ~NotificationHost()
        {
            // Désabonner l'événement pour éviter les fuites de mémoire
            NotificationService.OnNotificationRequested -= ShowNotification;
        }
    }
}