/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using TheXDS.MCART.Attributes;

namespace TheXDS.Proteus.Annotations
{

    /// <summary>
    /// Marca un método de elemento de interacción con un valor que indica
    /// el grupo de interacciones al cual pertenece.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class InteractionGroupAttribute : TextAttribute
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="InteractionGroupAttribute"/>.
        /// </summary>
        /// <param name="interactionGroup">
        /// Grupo de interacciones al cual el método pertenecerá.
        /// </param>
        public InteractionGroupAttribute(string interactionGroup) : base(interactionGroup) {}
    }
}