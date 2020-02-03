/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Globalization;
using System.Windows.Data;

namespace TheXDS.Proteus.ValueConverters
{
    public class MinusConv : IValueConverter
    {
        /// <summary>Convierte un valor.</summary>
        /// <param name="value">
        ///   Valor generado por el origen de enlace.
        /// </param>
        /// <param name="targetType">
        ///   El tipo de la propiedad del destino de enlace.
        /// </param>
        /// <param name="parameter">
        ///   Parámetro de convertidor que se va a usar.
        /// </param>
        /// <param name="culture">
        ///   Referencia cultural que se va a usar en el convertidor.
        /// </param>
        /// <returns>
        ///   Valor convertido.
        ///    Si el método devuelve <see langword="null" />, se usa el valor nulo válido.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double) value - double.Parse((string)parameter);
        }

        /// <summary>Convierte un valor.</summary>
        /// <param name="value">
        ///   Valor generado por el destino de enlace.
        /// </param>
        /// <param name="targetType">Tipo al que se va a convertir.</param>
        /// <param name="parameter">
        ///   Parámetro de convertidor que se va a usar.
        /// </param>
        /// <param name="culture">
        ///   Referencia cultural que se va a usar en el convertidor.
        /// </param>
        /// <returns>
        ///   Valor convertido.
        ///    Si el método devuelve <see langword="null" />, se usa el valor nulo válido.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value + (double)parameter;
        }
    }
}