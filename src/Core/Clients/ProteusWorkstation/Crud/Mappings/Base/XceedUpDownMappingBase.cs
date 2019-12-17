/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Base;
using System;
using System.Windows;
using TheXDS.MCART.ViewModel;
using TheXDS.MCART.Math;
using Xceed.Wpf.Toolkit.Primitives;

namespace TheXDS.Proteus.Crud.Mappings.Base
{
    public abstract class XceedUpDownMappingBase<T> : PropertyMapping where T : struct, IComparable<T>
    {
        protected static readonly DependencyProperty _prop;
        static XceedUpDownMappingBase()
        {
            _prop = UpDownBase<T?>.ValueProperty;
        }

        public XceedUpDownMappingBase(IPropertyDescription property, FrameworkElement control) : base(property, control)
        {
        }

        protected void Control_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var _control = sender as FrameworkElement;

            var m = Description.PropertySource == PropertyLocation.Model 
                ? (_control.DataContext as IDynamicViewModel)?.Entity 
                : _control.DataContext;

            if (m is null) return;
            SetValue(m);
        }
        protected T? Get(UpDownBase<T?> upDown) => (T?)upDown.GetValue(_prop);
        protected void Set(UpDownBase<T?> upDown, object value)
        {
            upDown.SetValue(_prop, value is T v ? v.Clamp(upDown.Minimum ?? v, upDown.Maximum ?? v) : (T?)null);
        }
    }
}