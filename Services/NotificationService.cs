﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace GetOutAdminV2.Services
{
    public enum NotificationType
    {
        Success,
        Error,
        Warning,
        Info
    }

    public static class NotificationService
    {
        // Événement auquel les vues peuvent s'abonner
        public static event Action<string, NotificationType, int>? OnNotificationRequested;

        // Méthode pour déclencher une notification
        public static void Notify(string message, NotificationType type = NotificationType.Info, int durationInSeconds = 5)
        {
            if (OnNotificationRequested != null)
            {
                OnNotificationRequested.Invoke(message, type, durationInSeconds);
            }
            else
            {
                // Fallback au cas où personne n'est abonné à l'événement
                MessageBox.Show(message, GetTitleForType(type), MessageBoxButton.OK, GetImageForType(type));
            }
        }

        private static string GetTitleForType(NotificationType type)
        {
            return type switch
            {
                NotificationType.Success => "Succès",
                NotificationType.Error => "Erreur",
                NotificationType.Warning => "Avertissement",
                NotificationType.Info => "Information",
                _ => "Notification"
            };
        }

        private static MessageBoxImage GetImageForType(NotificationType type)
        {
            return type switch
            {
                NotificationType.Success => MessageBoxImage.Information,
                NotificationType.Error => MessageBoxImage.Error,
                NotificationType.Warning => MessageBoxImage.Warning,
                NotificationType.Info => MessageBoxImage.Information,
                _ => MessageBoxImage.None
            };
        }

        // Méthode pour créer un contrôle de notification
        public static Border CreateNotification(string message, NotificationType type, int durationInSeconds)
        {
            try
            {
                // Créer le conteneur
                var border = new Border
                {
                    CornerRadius = new CornerRadius(5),
                    Padding = new Thickness(15, 10, 15, 10),
                    Margin = new Thickness(10, 5, 10, 5),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Top,
                    MaxWidth = 500,
                    Opacity = 0
                };

                // Définir le style en fonction du type
                switch (type)
                {
                    case NotificationType.Success:
                        border.Background = new SolidColorBrush(Color.FromRgb(39, 174, 96));
                        break;
                    case NotificationType.Error:
                        border.Background = new SolidColorBrush(Color.FromRgb(231, 76, 60));
                        break;
                    case NotificationType.Warning:
                        border.Background = new SolidColorBrush(Color.FromRgb(241, 196, 15));
                        break;
                    case NotificationType.Info:
                    default:
                        border.Background = new SolidColorBrush(Color.FromRgb(52, 152, 219));
                        break;
                }

                // Créer le texte
                var textBlock = new TextBlock
                {
                    Text = message,
                    Foreground = new SolidColorBrush(Colors.White),
                    TextWrapping = TextWrapping.Wrap,
                    FontSize = 14,
                    FontWeight = FontWeights.SemiBold
                };

                // Assembler le tout
                border.Child = textBlock;

                // Animation d'entrée
                var fadeInAnimation = new DoubleAnimation
                {
                    From = 0,
                    To = 1,
                    Duration = TimeSpan.FromSeconds(0.3)
                };

                // Animation de sortie
                var fadeOutAnimation = new DoubleAnimation
                {
                    From = 1,
                    To = 0,
                    Duration = TimeSpan.FromSeconds(0.3),
                    BeginTime = TimeSpan.FromSeconds(durationInSeconds)
                };

                var storyboard = new Storyboard();
                Storyboard.SetTarget(fadeInAnimation, border);
                Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath(UIElement.OpacityProperty));
                storyboard.Children.Add(fadeInAnimation);

                Storyboard.SetTarget(fadeOutAnimation, border);
                Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath(UIElement.OpacityProperty));
                storyboard.Children.Add(fadeOutAnimation);

                // Supprimer la notification après l'animation
                fadeOutAnimation.Completed += (s, e) =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (border.Parent is Panel panel)
                        {
                            panel.Children.Remove(border);
                        }
                        else if (border.Parent is ItemsControl itemsControl)
                        {
                            itemsControl.Items.Remove(border);
                        }
                    });
                };

                // Démarrer les animations
                border.Loaded += (s, e) => storyboard.Begin();

                return border;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors de la création de la notification: {ex.Message}");
                MessageBox.Show(message, GetTitleForType(type), MessageBoxButton.OK, GetImageForType(type));

                // Retourner une bordure vide en cas d'erreur pour éviter une exception
                return new Border();
            }
        }
    }
}