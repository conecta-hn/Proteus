/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Plugins;
using System;
using TheXDS.MCART.Attributes;
using Xceed.Wpf.Toolkit;

namespace XceedPatches
{
    /// <summary>
    /// Aplica parches a controles DateTime de Xceed Wpf Toolkit 3.5.0
    /// </summary>
    [Description("Aplica parches a controles DateTime de Xceed Wpf Toolkit 3.5.0")]
    public class DateTimeControlsPatch : Patch<DateTimeUpDown>
    {
        /// <summary>
        /// Aplica un parche personalizado a un control 
        /// <see cref="DateTimeUpDown"/>.
        /// </summary>
        /// <param name="d">
        /// Control al cual aplicar el parche.
        /// </param>
        public override void Apply(DateTimeUpDown d)
        {
            d.Minimum = new DateTime(1901, 1, 1);
            d.DefaultValue = DateTime.Now;
            if (d.Value == DateTime.MinValue)
            {
                d.Value = DateTime.Now;
            }
            d.ValueChanged += BoundToValidDate;
        }

        private void BoundToValidDate(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            var d = (DateTimeUpDown)sender;
            if (d.Value < new DateTime(1901, 1, 1))
            {
                e.Handled = true;
                d.Value = DateTime.Now;
            }
        }
    }
}