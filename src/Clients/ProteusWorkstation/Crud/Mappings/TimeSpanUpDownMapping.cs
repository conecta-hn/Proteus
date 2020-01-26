/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Crud.Mappings.Base;
using Xceed.Wpf.Toolkit;

namespace TheXDS.Proteus.Crud.Mappings
{
    public class TimeSpanUpDownMapping : XceedUpDownMapping<TimeSpanUpDown, TimeSpan>
    {
        public TimeSpanUpDownMapping(IPropertyDescription property) : base(property)
        {
        }
    }
    public class TimeSpanUpDownRangeMapping : XceedUpDownRangeMapping<TimeSpanUpDown, TimeSpan>
    {
        public TimeSpanUpDownRangeMapping(IPropertyDescription property) : base(property)
        {
        }
    }

    public class TimeUpDownMapping : PropertyMapping
    {
        protected new DateTimeUpDown Control => (DateTimeUpDown)base.Control;
        public TimeUpDownMapping(IPropertyDescription property) : base(property, new DateTimeUpDown())
        {
            Control.Format = DateTimeFormat.ShortTime;
        }

        public override object ControlValue
        {
            get => Control.Value?.TimeOfDay;
            set => Control.Value = (value as TimeSpan?).HasValue ? (DateTime.MinValue + ((value as TimeSpan?) ?? default)) : (DateTime?)null;
        }

        public override void ClearControlValue()
        {
            ControlValue = Description.Default;
        }
    }
}