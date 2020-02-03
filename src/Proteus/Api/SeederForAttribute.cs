/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using TheXDS.MCART.Attributes;
using TheXDS.MCART.Exceptions;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Proteus.Api
{
    /// <summary>
    /// Indica que una clase es un Seeder de base de datos.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class SeederForAttribute : TypeAttribute
    {
        /// <inheritdoc />
        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="TypeAttribute" />.
        /// </summary>
        /// <param name="type">Valor de este atributo.</param>
        public SeederForAttribute(Type type) : base(type)
        {
            if (!type.Implements<Service>()) throw new InvalidTypeException(type);
        }
    }
}
