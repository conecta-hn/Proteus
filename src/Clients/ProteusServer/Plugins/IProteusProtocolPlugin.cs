/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Protocols;
using System;
using TheXDS.MCART.Networking.Legacy.Server;

namespace TheXDS.Proteus.Plugins
{
    /// <summary>
    /// Expone una serie de métodos a implementar por un Plugin que exponga
    /// un protocolo de redes de MCART al servidor de Proteus.
    /// </summary>
    public interface IProteusProtocolPlugin<T> : IProteusProtocolPlugin, IProteusProtocol<T> where T : Client
    {
        /// <summary>
        /// Obtiene la instancia asociada del protocolo ofrecido por este
        /// plugin.
        /// </summary>
        IProtocol<T> Protocol { get; }
    }

    public interface IProteusProtocolPlugin : IProteusProtocol
    {
        /// <summary>
        /// Obtiene el tipo de clientes que el protocolo de este plugin
        /// atiende.
        /// </summary>
        Type ClientType { get; }

        /// <summary>
        /// Obtiene el tipo del protocolo ofrecido por este plugin.
        /// </summary>
        Type ProtocolType { get; }
    }
}