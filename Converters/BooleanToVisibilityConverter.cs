using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace GetOutAdminV2.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool visibility = (bool)value;

            // Si un paramètre "inverse" est spécifié, on inverse la logique
            if (parameter != null && parameter.ToString().ToLower() == "inverse")
            {
                visibility = !visibility;
            }

            return visibility ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visibility = (Visibility)value;

            bool result = visibility == Visibility.Visible;

            // Si un paramètre "inverse" est spécifié, on inverse la logique
            if (parameter != null && parameter.ToString().ToLower() == "inverse")
            {
                result = !result;
            }

            return result;
        }
    }
}
