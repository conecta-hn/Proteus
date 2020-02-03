/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Windows.Controls;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Crud.Mappings.Base;
using Xceed.Wpf.Toolkit;

namespace TheXDS.Proteus.Crud.Mappings
{
    public class DateTimeUpDownMapping : XceedUpDownMapping<DateTimePicker, DateTime>
    {
        public DateTimeUpDownMapping(IPropertyDescription property) : base(property)
        {
            if (property is IPropertyDateDescription d)
            {
                Control.Format = d.WithTime ? DateTimeFormat.FullDateTime : DateTimeFormat.LongDate;
            }
        }
    }
}