using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace GetOutAdminV2.Converters
{
    public class NavBarMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Si IsNotLogInPage est true, appliquez une marge à gauche pour la barre de navigation
            if (value is bool isNotLogInPage && isNotLogInPage)
            {
                return new Thickness(260, 0, 0, 0); // Marge à gauche de 260 (largeur de la barre de navigation)
            }

            // Sinon, pas de marge
            return new Thickness(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
