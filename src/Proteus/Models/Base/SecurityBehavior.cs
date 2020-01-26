/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;

namespace TheXDS.Proteus.Models.Base
{
    /// <summary>
    /// Enumera los posibles valores que describen un comportamiento de
    /// seguridad dentro de Proteus.
    /// </summary>
    [Flags]
    public enum SecurityBehavior
    {
        /// <summary>
        /// Componente bloqueado.
        /// </summary>
        Locked,

        /// <summary>
        /// Componente visible.
        /// </summary>
        Visible,

        /// <summary>
        /// Componente habilitado.
        /// </summary>
        Enabled,

        /// <summary>
        /// Componente desbloqueado.
        /// </summary>
        Unlocked,
    }
}