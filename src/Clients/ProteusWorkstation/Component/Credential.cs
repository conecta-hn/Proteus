/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Config;
using System.Security;
using TheXDS.MCART.Security.Password;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Proteus.Component
{
    /// <summary>
    /// Describe una credencial básica.
    /// </summary>
    public struct Credential : ICredential
    {
        /// <summary>
        /// Nombre del usuario.
        /// </summary>
        public string? Username { get; }

        /// <summary>
        /// Contraseña del usuario.
        /// </summary>
        public SecureString? Password { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la estructura
        /// <see cref="Credential"/>.
        /// </summary>
        /// <param name="user">Nombre de usuario.</param>
        /// <param name="pass">Contraseña.</param>
        public Credential(string user, SecureString? pass)
        {
            Username = user;
            Password = pass;
        }

        /// <summary>
        /// Inicializa una nueva instancia de la estructura
        /// <see cref="Credential"/> sin contraseña.
        /// </summary>
        /// <param name="user">Nombre de usuario.</param>
        public Credential(string? user)
        {
            Username = user;
            Password = null;
        }

        /// <summary>
        /// Obtiene una credencial desde la configuración local.
        /// </summary>
        /// <returns></returns>
        public static Credential FromSettings()
        {
            var u = Settings.Default.LastLogin;
            SecureString? p = null;
            if (Settings.Default.RememberPassword)
            {
                p = Settings.Default.SavedPassword.ToSecureString();
            }
            return new Credential(u, p);
        }
    }
}