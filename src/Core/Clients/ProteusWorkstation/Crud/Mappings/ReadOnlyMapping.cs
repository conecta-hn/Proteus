/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using TheXDS.Proteus.Crud.Base;
using TheXDS.MCART.Types.Extensions;
using static TheXDS.MCART.Types.Extensions.StringExtensions;

namespace TheXDS.Proteus.Crud.Mappings
{
    /// <summary>
    /// Mapeo de propiedades a mostrar en una página auto-generada de CRUD
    /// que son de sólo lectura.
    /// </summary>
    public sealed class ReadOnlyMapping : IPropertyMapping
    {
        /// <summary>
        /// Crea un control equivalente al generado por este mapping, útil
        /// para mostrar propiedades en la UI aún cuando un mapping falla.
        /// </summary>
        /// <param name="property">
        /// Objeto que describe la propiedad a mapear.
        /// </param>
        /// <returns>
        /// Un <see cref="TextBlock"/> con contenido equivalente al
        /// generado por este <see cref="IPropertyMapping"/>.
        /// </returns>
        public static FrameworkElement CreateReadOnlyControl(IPropertyDescription property)
        {
            var tb = new TextBlock();
            CreateReadOnlyControl(tb, property);
            return tb;
        }

        private static void CreateReadOnlyControl(TextBlock tb, IPropertyDescription property)
        {
            if (property is null)
            {
                tb.Text = "❌ Se intentó mapear un objeto null.";
                return;
            }

            var run = new Run();
            var rop = new ReadOnlyPresenter();

            if (property is ILinkPropertyDescription link)
            {
                rop._linkResolutionType = link.Model;
            }

            var b = new Binding(property.GetBindingString())
            {
                Mode = BindingMode.OneWay,
                StringFormat = property.ReadOnlyFormat,
                Converter = rop
            };
            run.SetBinding(Run.TextProperty, b);

            tb.Inlines.Add($"{property.Label}: ");
            tb.Inlines.Add(run);
            tb.ToolTip = property.Tooltip.OrNull() ?? property.Label;
            return;
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="ReadOnlyMapping"/>.
        /// </summary>
        /// <param name="property">
        /// Objeto que describe la propiedad a mapear.
        /// </param>
        public ReadOnlyMapping(IPropertyDescription property)
        {
            Description = property;
            Property = property.Property;

            if (property is IPropertyTextDescription td && td.Kind == TextKind.PicturePath)
            {
                var i = new Image();

                i.SetBinding(Image.SourceProperty, new Binding(property.GetBindingString())
                {
                    Mode = BindingMode.OneWay,
                    Converter = new ImageLoader()
                });

                Control = i;
            }
            else
            {
                Control = new TextBlock();
                CreateReadOnlyControl((TextBlock)Control, property);
            }
        }

        /// <summary>
        /// Obtiene la descripción utilizada para construir este
        /// <see cref="IPropertyMapping"/>.
        /// </summary>
        public IPropertyDescription Description { get; }

        /// <summary>
        /// Obtiene o establece el valor del control de forma manual.
        /// </summary>
        public object? ControlValue
        {
            get
            {
                return Control switch
                {
                    TextBlock tb => tb.Text,
                    _ => null
                };
            }
            set { }
        }

        /// <summary>
        /// Propiedad controlada por esta instancia de
        /// <see cref="IPropertyMapping"/>.
        /// </summary>
        public PropertyInfo Property { get; }

        /// <summary>
        /// Control de edición asociado a esta instancia de
        /// <see cref="IPropertyMapping"/>.
        /// </summary>
        public FrameworkElement Control { get; }

        /// <summary>
        /// Obtiene al control contenedor del control generado por este
        /// <see cref="IPropertyMapping"/>, o al mismo.
        /// </summary>
        public FrameworkElement ContainingControl => Control;

        /// <summary>
        /// Limpia el valor del control e edición asociado a este
        /// <see cref="IPropertyMapping"/>.
        /// </summary>
        public void ClearControlValue()
        {
        }

        /// <summary>
        /// Obtiene el valor de la propiedad asociada a este
        /// <see cref="IPropertyMapping"/> desde la instancia de objet
        /// especificada.
        /// </summary>
        /// <param name="instance"></param>
        public void GetValue(object instance)
        {
            //ControlValue = Property.GetMethod.Invoke(instance, new object[] { });
        }

        /// <summary>
        /// Establece el valor de la propiedad de la instancia especificada
        /// utilizando el valor obtenido desde el control asociado a este
        /// <see cref="IPropertyMapping"/>.
        /// </summary>
        /// <param name="instance">
        /// Instancia de destino para el valor obtenido desde el control.
        /// </param>
        public void SetValue(object instance)
        {
        }
    }
}