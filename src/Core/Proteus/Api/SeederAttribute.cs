using System;
using TheXDS.MCART.Attributes;

namespace TheXDS.Proteus.Api
{
    /// <summary>
    ///     Indica que una clase es un Seeder de base de datos.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class SeederAttribute : TypeAttribute
    {
        /// <inheritdoc />
        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="TypeAttribute" />.
        /// </summary>
        /// <param name="type">Valor de este atributo.</param>
        public SeederAttribute(Type type) : base(type)
        {
        }
    }
}
