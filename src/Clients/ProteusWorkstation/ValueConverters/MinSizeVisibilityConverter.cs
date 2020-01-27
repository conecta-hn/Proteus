/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TheXDS.Proteus.ValueConverters
{
    public class MinSizeVisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double d && double.TryParse(parameter?.ToString(), out var l))
            {
                return d < l ? Visibility.Collapsed : Visibility.Visible;
            }
            else
                return Visibility.Visible;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Visibility v && v == Visibility.Visible ? parameter : 0.0;
        }
    }
}