/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using TheXDS.MCART.Attributes;

namespace TheXDS.Proteus.Api
{
    /// <summary>
    /// Atributo que describe banderas de seguridad a aplicar al método que ha
    /// sido marcado.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class MethodKindAttribute : Attribute, IValueAttribute<SecurityFlags>
    {
        /// <summary>
        /// Obtiene las banderas de seguridad con las que un método ha sido
        /// marcado.
        /// </summary>
        public SecurityFlags Value { get; }

        /// <summary>
        /// Marca un método para requerir las banderas de seguridad
        /// especificadas al ejecutarse en un contexto de servicio.
        /// </summary>
        /// <param name="kind">
        /// Banderas de seguridad con las que se marcará un método.
        /// </param>
        public MethodKindAttribute(SecurityFlags kind)
        {
            Value = kind;
        }
    }
}
