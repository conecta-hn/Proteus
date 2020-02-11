/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.MCART.Types.Base;

namespace TheXDS.Proteus.Models.Base
{
    /// <summary>
    /// Define una serie de miembros a implementar por un tipo que
    /// represente a un usuario.
    /// </summary>
    public interface IUser : INameable
    {
        /// <summary>
        /// Obtiene el Id del usuario como un <see cref="string"/>.
        /// </summary>
        string StringId { get; }

        /// <summary>
        /// Obtiene un valor que indica si el usuario se encuentra
        /// habilitado.
        /// </summary>
        bool Enabled { get; }

        /// <summary>
        /// Obtiene una referencia al Hash de la contraseña del usuario.
        /// </summary>
        byte[] PasswordHash { get; set; }

        /// <summary>
        /// Obtiene un valor que indica si el usuario tiene pendiente una 
        /// solicitud de cambio de contraseña.
        /// </summary>
        bool ScheduledPasswordChange { get; set; }
    }
}