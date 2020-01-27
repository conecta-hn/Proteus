/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Globalization;
using System.Windows.Data;
using static TheXDS.MCART.WpfUi;

namespace TheXDS.Proteus.Crud.Mappings
{
    public class ImageLoader : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Uri.TryCreate(value?.ToString(), UriKind.Absolute, out var u) 
                ? GetBitmap(u) 
                : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }
}