/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.MCART.Networking.Server;

namespace TheXDS.Proteus.Protocols
{
    public interface IProteusProtocol
    {
        /// <summary>
        /// Construye un nuevo servidor utilizando el protocolo ofrecido
        /// por este plugin.
        /// </summary>
        /// <returns>
        /// Un nuevo servidor utilizando el protocolo ofrecido por este
        /// plugin.
        /// </returns>
        Server BuildServer();
    }
}