/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Globalization;
using System.Windows.Data;

namespace TheXDS.Proteus.ValueConverters
{
    public class TimeSpanDaysConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan ts) return (short)ts.TotalDays;
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is short v) return TimeSpan.FromDays(v);
            return default(TimeSpan);
        }
    }
}