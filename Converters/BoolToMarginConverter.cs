using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace GetOutAdminV2.Converters
{
public class BoolToMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isVisible)
            {
                return isVisible ? new Thickness(260, 0, 10, 10) : new Thickness(10, 0, 10, 10);
            }
            return new Thickness(10, 0, 10, 10);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Thickness)value).Left > 100; // Si Left > 100, alors la navbar est visible
        }
    }
}