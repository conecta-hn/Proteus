/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace TheXDS.Proteus.ValueConverters
{
    public class AnyContentVisibilityConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case ContentControl contentControl:
                    return (contentControl.Content as FrameworkElement)?.Visibility ?? Visibility.Collapsed;
                case Panel panel:
                    foreach (var j in panel.Children)
                    {
                        if ((j as FrameworkElement)?.Visibility==Visibility.Visible) return Visibility.Visible;
                    }
                    return Visibility.Collapsed;
                default:
                    return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }
}