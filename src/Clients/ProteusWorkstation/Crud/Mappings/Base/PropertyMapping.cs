/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using TheXDS.MCART;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Widgets;
using static TheXDS.MCART.Types.Extensions.StringExtensions;

namespace TheXDS.Proteus.Crud.Mappings.Base
{
    /// <summary>
    /// Clase base que describe todas las instancias de controles de
    /// edición para propiedades de objetos dentro de una página de CRUD
    /// auto-generada.
    /// </summary>
    public abstract class PropertyMapping : IPropertyMapping
    {
        private readonly FrameworkElement? _containingControl;

        /// <summary>
        /// Obtiene al control contenedor del control generado por este
        /// <see cref="IPropertyMapping"/>, o al mismo.
        /// </summary>
        public FrameworkElement ContainingControl => _containingControl ?? Control;

        /// <summary>
        /// Obtiene la descripción utilizada para construir este
        /// <see cref="PropertyMapping"/>.
        /// </summary>
        public IPropertyDescription Description { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="PropertyMapping"/>.
        /// </summary>
        /// <param name="property">
        /// Objeto que describe la propiedad a mapear.
        /// </param>
        /// <param name="control">
        /// Control a asociar a este <see cref="PropertyMapping"/>.
        /// </param>
        protected PropertyMapping(IPropertyDescription property, FrameworkElement control)
        {
            App.ApplyPatches(control);
            Description = property;
            Property = property.Property;
            Control = control;
            switch (property.Nullability)
            {
                case NullMode.Infer:
                    var pt = property.Property.PropertyType;
                    if (pt.IsValueType || pt.HasAttr<RequiredAttribute>())
                        goto case NullMode.Required;
                    goto case NullMode.Nullable;
                case NullMode.Required:
                    _containingControl = null;
                    break;
                case NullMode.Nullable:
                    _containingControl = NewToggleButton<CheckBox>();
                    break;
                case NullMode.Radio:
                    _containingControl = NewToggleButton<RadioButton>();
                    break;
            }
            Control.ToolTip = property.Tooltip;
            ProteusProp.SetWatermark(control, property.Label.OrNull() ?? property.Property.Name);
            ProteusProp.SetWatermarkAlwaysVisible(control, true);
            if (!property.Icon.IsEmpty()) ProteusProp.SetIcon(control, property.Icon);

            if (!(property.CustomBindings is null))
            {
                foreach (var j in property.CustomBindings)
                { 
                    control.SetBinding(j.Key, j.Value);
                }
            }
        }

        /// <summary>
        /// Aplica un workaround para el comportamiento errático de los botones
        /// de tipo <see cref="ToggleButton"/> de WPF, en el cual
        /// indeseablemente observan las pulsaciones de las teclas '+' y '-'.
        /// </summary>
        /// <param name="btn">Botón al cual aplicar el parche.</param>
        /// <remarks>
        /// Este método no se implementa como un <see cref="Plugins.Patch"/>
        /// porque los botones a parchar son controles de nulabilidad, los
        /// cuales por la naturaleza del generador no participan en el
        /// mecanismo de parcheo.
        /// </remarks>
        private void PatchToggleButton(ToggleButton btn)
        {
            btn.IsThreeState = true;
            btn.Click += (_, e) =>
            {
                if (btn.IsChecked is null) btn.IsChecked = false;
            };
        }

        private T NewToggleButton<T>() where T : ToggleButton, new()
        {
            var btn = new T()
            {
                Content = Control,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Top,
                Margin = new Thickness(10, 0, 0, 0)
            };
            PatchToggleButton(btn);
            Control.SetBinding(UIElement.IsEnabledProperty,
                new Binding()
                {
                    Source = btn,
                    Path = new PropertyPath(ToggleButton.IsCheckedProperty)
                });

            btn.Unchecked += (sender, e) =>
            {
                if (sender == e.OriginalSource) ClearControlValue();
            };
            return btn;
        }

        /// <summary>
        /// Propiedad controlada por esta instancia de
        /// <see cref="PropertyMapping"/>.
        /// </summary>
        public PropertyInfo Property { get; }

        /// <summary>
        /// Control de edición asociado a esta instancia de
        /// <see cref="PropertyMapping"/>.
        /// </summary>
        public FrameworkElement Control { get; }

        /// <summary>
        /// Obtiene o establece el valor del control de forma manual.
        /// </summary>
        public abstract object? ControlValue { get; set; }

        /// <summary>
        /// Limpia el valor del control e edición asociado a este
        /// <see cref="PropertyMapping"/>.
        /// </summary>
        public abstract void ClearControlValue();

        /// <summary>
        /// Obtiene el valor de la propiedad asociada a este
        /// <see cref="PropertyMapping"/> desde la instancia de objet
        /// especificada.
        /// </summary>
        /// <param name="instance"></param>
        public virtual void GetValue(object instance)
        {
            ControlValue = Property.GetMethod!.Invoke(instance, System.Array.Empty<object>());
            if (Control.Parent is ToggleButton tb)
            {
                tb.IsChecked = !(ControlValue is null);
            }
        }

        /// <summary>
        /// Establece el valor de la propiedad de la instancia especificada
        /// utilizando el valor obtenido desde el control asociado a este
        /// <see cref="PropertyMapping"/>.
        /// </summary>
        /// <param name="instance">
        /// Instancia de destino para el valor obtenido desde el control.
        /// </param>
        public virtual void SetValue(object instance)
        {
            if (ControlValue is IEnumerable<Models.Base.ModelBase> v)
            {
                if (!(Property.GetMethod!.Invoke(instance, System.Array.Empty<object>()) is IList c))
                {
                    c = Property.PropertyType.New<IList>();
                    Property.SetMethod?.Invoke(instance, new[] { c });
                }
                c.Clear();
                foreach (var j in v) c.Add(j);
            }
            else
            {
                Property.SetMethod?.Invoke(instance, new[] { ControlValue });
            }
        }
    }
}