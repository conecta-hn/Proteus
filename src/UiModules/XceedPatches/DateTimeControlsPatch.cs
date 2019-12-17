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
    [Description("Aplica parches a controles DateTime de Xceed Wpf Toolkit 3.5.0")]
    public class DateTimeControlsPatch : Patch<DateTimeUpDown>
    {
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
            var d = sender as DateTimeUpDown;
            if (d.Value < new DateTime(1901, 1, 1))
            {
                e.Handled = true;
                d.Value = DateTime.Now;
            }
        }
    }
}
