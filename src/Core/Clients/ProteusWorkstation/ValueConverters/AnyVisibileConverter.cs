/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace TheXDS.Proteus.ValueConverters
{
    public class AnyVisibileConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.OfType<Visibility>().Any(p => p == Visibility.Visible) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }
}