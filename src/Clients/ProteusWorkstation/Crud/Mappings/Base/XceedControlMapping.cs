/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Base;
using System;
using Xceed.Wpf.Toolkit.Primitives;
using static TheXDS.MCART.Types.Extensions.TypeExtensions;

namespace TheXDS.Proteus.Crud.Mappings.Base
{
    public abstract class XceedUpDownMapping<TControl, T> : XceedUpDownMappingBase<T> where TControl : UpDownBase<T?>, new() where T : struct, IComparable<T>
    {
        public XceedUpDownMapping(IPropertyDescription property) : base(property, new TControl())
        {
            _control = Control;
            _control.ValueChanged += Control_ValueChanged;
            _control.DisplayDefaultValueOnEmptyText = !property.PropertyType.IsClass;

            if (property is IPropertyNumberDescription<T> d)
            {
                if (!(d.Range is null))
                {
                    _control.Minimum = d.Range.Value.Minimum;
                    _control.Maximum = d.Range.Value.Maximum;
                }
            }
        }

        private readonly TControl _control;
        public new TControl Control => (TControl)base.Control;

        public override void ClearControlValue() => ControlValue = Description.Default ?? Property.PropertyType.Default();
        public override object ControlValue
        {
            get => Get(_control);
            set => Set(_control, value);
        }
    }
}