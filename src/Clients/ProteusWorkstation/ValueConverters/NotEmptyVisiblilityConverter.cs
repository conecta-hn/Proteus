/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using static TheXDS.MCART.Types.Extensions.StringExtensions;

namespace TheXDS.Proteus.ValueConverters
{
    public class NotEmptyVisiblilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is string s ? (object)(!s.IsEmpty() ? Visibility.Visible : Visibility.Collapsed) : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value as Visibility?)
            {
                case Visibility.Collapsed:
                    return Visibility.Visible.ToString();
                case Visibility.Visible:
                    return string.Empty;
                default:
                    return null;
            }
        }
    }
}