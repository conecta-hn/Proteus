﻿/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Windows;
using System.Windows.Data;
using TheXDS.Proteus.Crud.Base;
using static TheXDS.MCART.Types.Extensions.TypeExtensions;

namespace TheXDS.Proteus.Crud.Mappings.Base
{
    public abstract class ControlBindMapping<T> : PropertyMapping where T : FrameworkElement, new()
    {
        public ControlBindMapping(IPropertyDescription p, DependencyProperty bindableProperty) : base(p, new T())
        {
            _control = Control;
            _prop = bindableProperty;
            _control.SetBinding(_prop, new Binding(p.Property.Name));
        }
        public new T Control => (T)base.Control;
        private readonly T _control;
        private readonly DependencyProperty _prop;
        public override void ClearControlValue() => ControlValue = Property.PropertyType.Default();
        public override object ControlValue
        {
            get => _control.GetValue(_prop);
            set { }
        }
    }
}