/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Reflection;
using System.Windows;

namespace TheXDS.Proteus.Crud.Base
{
    /// <summary>
    /// Describe una serie de miebros a implementar por una clase que
    /// permita generar controles con Bindings para editar propiedades
    /// en una ventana de WPF.
    /// </summary>
    public interface IPropertyMapping
    {
        /// <summary>
        /// Obtiene la descripción utilizada para construir este
        /// <see cref="IPropertyMapping"/>.
        /// </summary>
        IPropertyDescription Description { get; }

        /// <summary>
        /// Propiedad asociada a este <see cref="IPropertyMapping"/>.
        /// </summary>
        PropertyInfo Property { get; }

        /// <summary>
        /// Obtiene una referecia genérica al control generado por este
        /// <see cref="IPropertyMapping"/>.
        /// </summary>
        FrameworkElement Control { get; }

        /// <summary>
        /// Obtiene al control contenedor del control generado por este
        /// <see cref="IPropertyMapping"/>, o al mismo.
        /// </summary>
        FrameworkElement ContainingControl { get; }

        /// <summary>
        /// Obtiene o establece manualmente el valor de este control.
        /// </summary>
        object ControlValue { get; set; }

        /// <summary>
        /// Limpia el valor del control.
        /// </summary>
        void ClearControlValue();

        /// <summary>
        /// Obtiene el valor de una propiedad de una entidad de datos y
        /// establece ese valor en el control editable de este
        /// <see cref="IPropertyMapping"/>.
        /// </summary>
        /// <param name="instance">
        /// Modelo de datos desde el cual obtener el valor de la propiedad
        /// asociada a este  <see cref="IPropertyMapping"/>.
        /// </param>
        void GetValue(object instance);

        /// <summary>
        /// Obtiene el valor del control editable de este
        /// <see cref="IPropertyMapping"/> y lo establece en la propiedad 
        /// correspondiente del modelo de datos.
        /// </summary>
        /// <param name="instance">
        /// Modelo de datos en el cual establecer el valor de la propiedad
        /// asociada a este <see cref="IPropertyMapping"/>.
        /// </param>
        void SetValue(object instance);
    }
}