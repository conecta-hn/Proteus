/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Dialogs;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Component
{
    /// <summary>
    /// Elevador predeterminado que otorga todos los permisos de forma
    /// indiscriminada. Esta clase no debe utilizarse, y solo existe para
    /// propósitos de prueba y desarrollo.
    /// </summary>
    public class UacElevator : IElevator
    {
        /// <summary>
        /// Eleva los permisos de un objeto credencial que lo solicite.
        /// </summary>
        /// <param name="credential">
        /// Credencia que está solicitando la elevación de permisos.
        /// </param>
        /// <returns>
        /// Este método siempre devuelve <see langword="true"/>.
        /// </returns>
        public bool Elevate(ref IProteusUserCredential? credential)
        {
            if (credential?.Id == "root") return true;
            return UacSplash.Elevate(ref credential);
        }
    }
}