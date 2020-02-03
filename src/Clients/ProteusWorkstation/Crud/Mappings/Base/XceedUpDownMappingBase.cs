/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Base;
using System;
using System.Windows;
using TheXDS.MCART.ViewModel;
using TheXDS.MCART.Math;
using Xceed.Wpf.Toolkit.Primitives;
using TheXDS.MCART.Exceptions;

namespace TheXDS.Proteus.Crud.Mappings.Base
{
    /// <summary>
    /// Clase base de mapping de propiedad que permite utilizar controles de
    /// Xceed WPF Toolkit que implementan la clase <see cref="UpDownBase{T}"/>.
    /// </summary>
    /// <typeparam name="T">
    /// Tipo de valor aceptado por el control.
    /// </typeparam>
    public abstract class XceedUpDownMappingBase<T> : PropertyMapping where T : struct, IComparable<T>
    {
        /// <summary>
        /// Obtiene una referencia a la propiedad de dependencia asociada al
        /// valor del control.
        /// </summary>
        protected static readonly DependencyProperty _prop = UpDownBase<T?>.ValueProperty;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="XceedUpDownMappingBase{T}"/>.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="control"></param>
        protected XceedUpDownMappingBase(IPropertyDescription property, FrameworkElement control) : base(property, control)
        {
        }

        /// <summary>
        /// Método compatible con la firma del evento
        /// <see cref="UpDownBase{T}.ValueChanged"/> del control para el cual
        /// se implementa el <see cref="PropertyMapping"/>. Ayuda a manejar
        /// correctamente el cambio de valor del control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Control_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var _control = sender as FrameworkElement ?? throw new TamperException();
            var m = Description.PropertySource == PropertyLocation.Model 
                ? (_control.DataContext as IEntityViewModel)?.Entity 
                : _control.DataContext;

            if (m is null) return;
            SetValue(m);
        }

        /// <summary>
        /// Obtiene el valor asociado al control.
        /// </summary>
        /// <param name="upDown">
        /// Control desde el cual obtener el valor.
        /// </param>
        /// <returns>El valor del control.</returns>
        protected T? Get(UpDownBase<T?> upDown) => (T?)upDown.GetValue(_prop);

        /// <summary>
        /// Establece el valor asociado al control.
        /// </summary>
        /// <param name="upDown">
        /// Control al cual establecerle el valor.
        /// </param>
        /// <param name="value">Valor a establecer en el control.</param>
        protected void Set(UpDownBase<T?> upDown, object? value)
        {
            upDown.SetValue(_prop, value is T v ? v.Clamp(upDown.Minimum ?? v, upDown.Maximum ?? v) : (T?)null);
        }
    }
}