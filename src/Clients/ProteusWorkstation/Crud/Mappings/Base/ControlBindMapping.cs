/*
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
        private readonly BindingExpressionBase _binding;
        public ControlBindMapping(IPropertyDescription p, DependencyProperty bindableProperty) : base(p, new T())
        {
            _control = Control;
            _prop = bindableProperty;
            _binding = _control.SetBinding(_prop, new Binding(p.GetBindingString()));
        }
        public new T Control => (T)base.Control;
        private readonly T _control;
        private readonly DependencyProperty _prop;
        public override void ClearControlValue()
        {
            _control.SetCurrentValue(_prop, Description.UseDefault ? Description.Default : Property.PropertyType.Default());
            _binding.UpdateSource();
        }

        public override object? ControlValue
        {
            get => _control.GetValue(_prop);
            set { }
        }
    }
}