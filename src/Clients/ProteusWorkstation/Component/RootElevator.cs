/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.MCART.Attributes;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Component
{
    /// <summary>
    /// Elevador predeterminado que otorga todos los permisos de forma
    /// indiscriminada. Esta clase no debe utilizarse, y solo existe para
    /// propósitos de prueba y desarrollo.
    /// </summary>
    [Dangerous]
    public class RootElevator : IElevator
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
        public bool Elevate(ref IProteusUserCredential credential)
        {
            return true;
        }
    }
}