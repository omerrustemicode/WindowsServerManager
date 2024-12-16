using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ComputerManagment  // Ensure this matches your project namespace
{
    public class LengthToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Check if the string is not null and has a length greater than 0
            return value != null && value.ToString().Length > 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;  // Not needed for this converter
        }
    }
}
