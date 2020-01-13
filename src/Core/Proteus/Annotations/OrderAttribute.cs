/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using TheXDS.MCART.Attributes;

namespace TheXDS.Proteus.Component.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// Establece el orden de un campo a la hora de crear un Scaffold.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class OrderAttribute : IntAttribute
    {
        /// <inheritdoc />
        /// <summary>
        /// Establece el orden de un campo a la hora de crear un Scaffold.
        /// </summary>
        /// <param name="attributeValue">
        /// Valor ordinal del campo.
        /// </param>
        public OrderAttribute(int attributeValue) : base(attributeValue)
        {
        }
    }
}