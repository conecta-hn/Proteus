/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

#nullable enable

using TheXDS.Proteus.Models.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Config;

namespace TheXDS.Proteus.Crud.Mappings
{
    internal class ReadOnlyPresenter : IValueConverter
    {
        private const string _sep = "\n  • ";
        internal static Dictionary<Type, Func<ModelBase, string>> _customConversions = new Dictionary<Type, Func<ModelBase, string>>();
        
        internal Type? _linkResolutionType = null;
        private readonly bool _enumerateLists;

        public ReadOnlyPresenter() : this(true)
        {
        }

        public ReadOnlyPresenter(bool enumerateLists)
        {
            _enumerateLists = enumerateLists;
        }

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ToString(_linkResolutionType is null ? value : Proteus.ResolveLink(_linkResolutionType, value), culture);
        }

        public string? ToString(object? value)
        {
            return ToString(value, CultureInfo.CurrentCulture);
        }
        public string? ToString(object? value, CultureInfo? culture)
        {
            culture ??= CultureInfo.CreateSpecificCulture("es-HN");
            if (value is ModelBase m && _customConversions.ContainsKey(m.GetType()))
            {
                return _customConversions[m.GetType()](m);
            }
            return value switch
            {
                Enum n              => n.NameOf(),
                bool @bool          => @bool ? "Sí" : "No",
                decimal x           => x.ToString("C", culture),
                string @string      => @string,                
                IEnumerable c       => _enumerateLists ? Enumerate(c, culture) : $"{CountEnumerable(c)} elementos",
                null                => "N/A",
                _                   => value.ToString(),
            };
        }

        private string Enumerate(IEnumerable c, CultureInfo? culture)
        {
            culture ??= CultureInfo.CreateSpecificCulture("es-HN");
            var cc = c.ToGeneric().Take(Settings.Default.RowLimit).ToList();
            return cc.Any() ? $"{_sep}{string.Join(_sep, cc.Select(p => ToString(p, culture)).ToArray())}" : "sin datos.";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static int CountEnumerable(IEnumerable e)
        {
            var n = e.GetEnumerator();
            var c = 0;
            while (n.MoveNext()) c++;
            return c;
        }

    }
}