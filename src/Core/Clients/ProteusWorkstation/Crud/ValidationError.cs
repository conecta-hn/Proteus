/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Reflection;

namespace TheXDS.Proteus.Crud
{
    /// <summary>
    ///     Describe un error de validación.
    /// </summary>
    public class ValidationError
    {
        /// <inheritdoc />
        /// <summary>
        ///     Inicializa una nueva instancia de la clase <see cref="T:TheXDS.Proteus.Component.ValidationError" />.
        /// </summary>
        /// <param name="failedProperty">Propiedad que ha fallado.</param>
        public ValidationError(PropertyInfo failedProperty) : 
            this(failedProperty, "El valor de este campo es inválido.")
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     inicializa una nueva instancia de la clase <see cref="T:TheXDS.Proteus.Component.ValidationError" />.
        /// </summary>
        /// <param name="message">Mensaje de error.</param>
        public ValidationError(string message) : this(null, message)
        {
        }

        /// <summary>
        ///     Inicializa una nueva instancia de la clase <see cref="ValidationError" />.
        /// </summary>
        /// <param name="failedProperty">Propiedad que ha fallado.</param>
        /// <param name="message">Mensaje de error.</param>
        public ValidationError(PropertyInfo failedProperty, string message)
        {
            FailedProperty = failedProperty;
            Message = message;
        }

        /// <summary>
        ///     Propiedad que ha fallado.
        /// </summary>
        public PropertyInfo FailedProperty { get; }

        /// <summary>
        ///     Mensaje sobre el fallo.
        /// </summary>
        public string Message { get; }

        public static implicit operator ValidationError(string message) => new ValidationError(message);
    }

    /// <inheritdoc />
    /// <summary>
    ///     Describe un error de validación donde el campo no contiene un valor.
    /// </summary>
    public class NullValidationError : ValidationError
    {
        public NullValidationError(PropertyInfo failedProperty) : base(failedProperty, "Este campo es requerido.")
        {
        }

        public NullValidationError(PropertyInfo failedProperty, string message) : base(failedProperty, message)
        {
        }
    }
}