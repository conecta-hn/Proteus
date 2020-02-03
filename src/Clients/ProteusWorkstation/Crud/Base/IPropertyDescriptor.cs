/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models.Base;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;

namespace TheXDS.Proteus.Crud.Base
{
    /// <summary>
    /// Define una serie de miembros a implementar por una clase que
    /// permita describir la construcción de ventanas CRUD de una propiedad
    /// utilizando sintáxis Fluent.
    /// </summary>
    public interface IPropertyDescriptor
    {
        /// <summary>
        /// Referencia a la propiedad descrita por este
        /// <see cref="IPropertyDescriptor"/>.
        /// </summary>
        PropertyInfo Property { get; }

        /// <summary>
        /// Establece un valor predeterminado para el campo.
        /// </summary>
        /// <param name="value">Valor predeterminado a asignar.</param>
        /// <returns>
        /// Una referencia a la misma instancia para utilizar sintáxis
        /// Fluent.
        /// </returns>
        IPropertyDescriptor Default(object value);

        /// <summary>
        /// Indica que el campo no se mostrará en la interfaz del editor.
        /// </summary>
        void Hidden();

        /// <summary>
        /// Indica que una propiedad aparecerá en el Crud como elemento de
        /// solo lectura.
        /// </summary>
        void ReadOnly();

        /// <summary>
        /// Indica que una propiedad aparecerá en el Crud como elemento de
        /// solo lectura.
        /// </summary>
        /// <param name="format">
        /// Formato de texto a aplicar al control.
        /// </param>
        void ReadOnly(string format);

        /// <summary>
        /// Establece un ícono a aplicar a los controles que lo soporten.
        /// </summary>
        /// <param name="icon">Ícono a aplicar.</param>
        /// <returns>
        /// Una referencia a la misma instancia para utilizar sintáxis
        /// Fluent.
        /// </returns>
        IPropertyDescriptor Icon(char icon);

        /// <summary>
        /// Establece un ícono a aplicar a los controles que lo soporten.
        /// </summary>
        /// <param name="icon">Ícono a aplicar.</param>
        /// <returns>
        /// Una referencia a la misma instancia para utilizar sintáxis
        /// Fluent.
        /// </returns>
        IPropertyDescriptor Icon(string icon);

        /// <summary>
        /// Establece una etiqueta a aplicar a los controles que lo
        /// soporten.
        /// </summary>
        /// <param name="label">Etiqueta a aplicar.</param>
        /// <returns>
        /// Una referencia a la misma instancia para utilizar sintáxis
        /// Fluent.
        /// </returns>
        IPropertyDescriptor Label(string label);

        /// <summary>
        /// Establece el modo de nulidad de los campos.
        /// </summary>
        /// <param name="nullability">Modo de nulidad a establecer.</param>
        /// <returns>
        /// Una referencia a la misma instancia para utilizar sintáxis
        /// Fluent.
        /// </returns>
        IPropertyDescriptor Nullability(NullMode nullability);

        /// <summary>
        /// Indica que el campo es requerido, y por lo tanto no nulable.
        /// </summary>
        /// <returns>
        /// Una referencia a la misma instancia para utilizar sintáxis
        /// Fluent.
        /// </returns>
        IPropertyDescriptor Required();

        /// <summary>
        /// Indica que el campo es nulable, por lo que su control será
        /// contenido por un CheckBox.
        /// </summary>
        /// <returns>
        /// Una referencia a la misma instancia para utilizar sintáxis
        /// Fluent.
        /// </returns>
        IPropertyDescriptor Nullable();

        /// <summary>
        /// Indica que el campo es nulable, por lo que su control será
        /// contenido por un RadioButton.
        /// </summary>
        /// <returns>
        /// Una referencia a la misma instancia para utilizar sintáxis
        /// Fluent.
        /// </returns>
        IPropertyDescriptor RadioSelectable();

        /// <summary>
        /// Indica que el campo es nulable, por lo que su control será
        /// contenido por un CheckBox.
        /// </summary>
        /// <param name="groupId">
        /// Id del grupo de RadioButton al cual asociar al selector.
        /// </param>
        /// <returns>
        /// Una referencia a la misma instancia para utilizar sintáxis
        /// Fluent.
        /// </returns>
        IPropertyDescriptor RadioSelectable(string groupId);

        /// <summary>
        /// Establece un valor ordinal al campo.
        /// </summary>
        /// <param name="order">Valor ordinal del campo.</param>
        /// <returns>
        /// Una referencia a la misma instancia para utilizar sintáxis
        /// Fluent.
        /// </returns>
        IPropertyDescriptor Order(int order);

        /// <summary>
        /// Establece la función de validación de este campo.
        /// </summary>
        /// <param name="validator">
        /// Función de validación a utilizar.
        /// </param>
        /// <returns>
        /// Una referencia a la misma instancia para utilizar sintáxis
        /// Fluent.
        /// </returns>
        IPropertyDescriptor Validator(Func<ModelBase, PropertyInfo, IEnumerable<ValidationError>> validator);

        /// <summary>
        /// Establece la función de validación de este campo.
        /// </summary>
        /// <typeparam name="T">
        /// Modelo para el cual ejecutar la función de validación.
        /// </typeparam>
        /// <param name="validator">
        /// Función de validación a utilizar.
        /// </param>
        /// <returns>
        /// Una referencia a la misma instancia para utilizar sintáxis
        /// Fluent.
        /// </returns>
        IPropertyDescriptor Validator<T>(Func<T, PropertyInfo, IEnumerable<ValidationError>> validator) where T : ModelBase;

        /// <summary>
        /// Establece un texto de ayuda sobre los controles generados a
        /// partir de este descriptor.
        /// </summary>
        /// <param name="tooltip">Etiqueta a aplicar.</param>
        /// <returns>
        /// Una referencia a la misma instancia para utilizar sintáxis
        /// Fluent.
        /// </returns>
        IPropertyDescriptor Tooltip(string tooltip);

        /// <summary>
        /// Indica que el control mostrará la marca de agua siempre.
        /// </summary>
        /// <returns>
        /// Una referencia a la misma instancia para utilizar sintáxis
        /// Fluent.
        /// </returns>
        IPropertyDescriptor WatermarkAlwaysVisible();

        /// <summary>
        /// Agrega un nuevo <see cref="BindingBase"/> personalizado a
        /// aplicar al control generado.
        /// </summary>
        /// <param name="path">
        /// Propiedad del control a enlazar.
        /// </param>
        /// <param name="binding">
        /// Enlace de datos a utilizar.
        /// </param>
        /// <returns>
        /// Una referencia a la misma instancia para utilizar sintáxis
        /// Fluent.
        /// </returns>
        IPropertyDescriptor Bind(DependencyProperty path, BindingBase binding);

        /// <summary>
        /// Agrega un nuevo <see cref="BindingBase"/> personalizado a
        /// aplicar al control generado.
        /// </summary>
        /// <param name="path">
        /// Propiedad del control a enlazar.
        /// </param>
        /// <param name="binding">
        /// Ruta de la propiedad a enlazar.
        /// </param>
        /// <returns>
        /// Una referencia a la misma instancia para utilizar sintáxis
        /// Fluent.
        /// </returns>
        IPropertyDescriptor Bind(DependencyProperty path, string binding);

        /// <summary>
        /// Agrega un nuevo <see cref="BindingBase"/> personalizado a
        /// aplicar al control generado.
        /// </summary>
        /// <param name="path">
        /// Propiedad del control a enlazar.
        /// </param>
        /// <param name="binding">
        /// Ruta de la propiedad a enlazar.
        /// </param>
        /// <param name="source">
        /// Origen de datos del enlace.
        /// </param>
        /// <returns>
        /// Una referencia a la misma instancia para utilizar sintáxis
        /// Fluent.
        /// </returns>
        IPropertyDescriptor Bind(DependencyProperty path, string binding, object source);

        /// <summary>
        /// Agrega un nuevo <see cref="BindingBase"/> personalizado a
        /// aplicar al control generado.
        /// </summary>
        /// <param name="path">
        /// Propiedad del control a enlazar.
        /// </param>
        /// <param name="binding">
        /// Ruta de la propiedad a enlazar.
        /// </param>
        /// <returns>
        /// Una referencia a la misma instancia para utilizar sintáxis
        /// Fluent.
        /// </returns>
        IPropertyDescriptor Bind(DependencyProperty path, PropertyPath binding);

        /// <summary>
        /// Agrega un nuevo <see cref="BindingBase"/> personalizado a
        /// aplicar al control generado.
        /// </summary>
        /// <param name="path">
        /// Propiedad del control a enlazar.
        /// </param>
        /// <param name="binding">
        /// Ruta de la propiedad a enlazar.
        /// </param>
        /// <param name="source">
        /// Origen de datos del enlace.
        /// </param>
        /// <returns>
        /// Una referencia a la misma instancia para utilizar sintáxis
        /// Fluent.
        /// </returns>
        IPropertyDescriptor Bind(DependencyProperty path, PropertyPath binding, object source);

        /// <summary>
        /// Agrega esta propiedad como columna de lista al presentarse en
        /// un control <see cref="ListView"/>.
        /// </summary>
        /// <returns>
        /// Una referencia a la misma instancia para utilizar sintáxis
        /// Fluent.
        /// </returns>
        IPropertyDescriptor AsListColumn();

        /// <summary>
        /// Indica que la propiedad se mostrará en el panel autogenerado de
        /// detalles.
        /// </summary>
        /// <returns>
        /// Una referencia a la misma instancia para utilizar sintáxis
        /// Fluent.
        /// </returns>
        IPropertyDescriptor ShowInDetails();

        /// <summary>
        /// Establece un formato de presentación para una propiedad.
        /// </summary>
        /// <param name="format">Formato a aplicar.</param>
        /// <returns>
        /// Una referencia a la misma instancia para utilizar sintáxis
        /// Fluent.
        /// </returns>
        IPropertyDescriptor Format(string format);
    }
}