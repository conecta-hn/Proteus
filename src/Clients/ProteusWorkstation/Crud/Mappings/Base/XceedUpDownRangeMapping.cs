/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Base;
using System;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using TheXDS.MCART.Types;
using Xceed.Wpf.Toolkit.Primitives;

namespace TheXDS.Proteus.Crud.Mappings.Base
{
    /// <summary>
    /// Clase base para mapear rangos de valores por medio de controles de
    /// Xceed.
    /// </summary>
    /// <typeparam name="TControl">
    /// Tipo de control a utilizar para gestionar los valores del rango.
    /// </typeparam>
    /// <typeparam name="T">
    /// Tipo de valores del rango.
    /// </typeparam>
    public abstract class XceedUpDownRangeMapping<TControl, T> : XceedUpDownMappingBase<T> where TControl : UpDownBase<T?>, new() where T : struct, IComparable<T>
    {
        /// <summary>
        /// Obtiene el control que contieneel valor mínimo del rango.
        /// </summary>
        protected readonly TControl _lower = new TControl();

        /// <summary>
        /// Obtiene el control que contieneel valor máximo del rango.
        /// </summary>
        protected readonly TControl _upper = new TControl();

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="XceedUpDownRangeMapping{TControl, T}"/>.
        /// </summary>
        /// <param name="property"></param>
        public XceedUpDownRangeMapping(IPropertyDescription property) : base(property, new UniformGrid { Columns = 1 })
        {
            var grid = Control as UniformGrid;
            grid.Children.Add(_lower);
            grid.Children.Add(_upper);

            _lower.ValueChanged += Control_ValueChanged;
            _upper.ValueChanged += Control_ValueChanged;

            if (property is IPropertyNumberDescription<T> d)
            {
                if (!(d.Range is null))
                {
                    _lower.Minimum = d.Range.Value.Minimum;
                    _upper.Maximum = d.Range.Value.Maximum;
                }
                _lower.SetBinding(UpDownBase<T?>.MaximumProperty, new Binding("Value") { Source = _upper });
                _upper.SetBinding(UpDownBase<T?>.MinimumProperty, new Binding("Value") { Source = _lower });
            }
        }

        /// <summary>
        /// Obtiene o establece el valor contenido por el contorl de este
        /// <see cref="XceedUpDownRangeMapping{TControl, T}"/>.
        /// </summary>
        public override object ControlValue
        {
            get => new Range<T>(Get(_lower) ?? default, Get(_upper) ?? Get(_lower) ?? default);
            set
            {
                if (!(value is Range<T> range)) return;
                Set(_lower, range.Minimum);
                Set(_upper, range.Maximum);
            }
        }

        /// <summary>
        /// Limpia el valor contenido en este 
        /// <see cref="XceedUpDownRangeMapping{TControl, T}"/>.
        /// </summary>
        public override void ClearControlValue()
        {
            Set(_lower, null);
            Set(_upper, null);
        }
    }
}