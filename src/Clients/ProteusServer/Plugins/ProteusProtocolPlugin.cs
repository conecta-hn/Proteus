/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using TheXDS.MCART.Networking.Legacy.Server;
using TheXDS.MCART.PluginSupport.Legacy;

namespace TheXDS.Proteus.Plugins
{
    /// <inheritdoc cref="IProteusProtocolPlugin{T}" />
    /// <summary>
    /// Clase base para los plugins que ofrezcan protocolos de red para el
    /// servidor de Proteus.
    /// </summary>
    /// <typeparam name="TProtocol">Tipo de protocolo a exponer.</typeparam>
    /// <typeparam name="TClient">Tipo de cliente a atender.</typeparam>
    public abstract class ProteusProtocolPlugin<TProtocol, TClient>
        : Plugin, IProteusProtocolPlugin<TClient>
        where TProtocol : IProtocol<TClient>, new()
        where TClient : Client
    {
        /// <inheritdoc />
        /// <summary>
        /// Construye un nuevo servidor utilizando el protocolo ofrecido
        /// por este plugin.
        /// </summary>
        /// <returns>
        /// Un nuevo servidor utilizando el protocolo ofrecido por este
        /// plugin.
        /// </returns>
        public Server<TClient> BuildServer()
        {
            return new Server<TClient>(Protocol);
        }

        /// <inheritdoc />
        /// <summary>
        /// Obtiene el tipo de clientes que el protocolo de este plugin
        /// atiende.
        /// </summary>
        public Type ClientType => typeof(TClient);

        /// <inheritdoc />
        /// <summary>
        /// Obtiene la instancia asociada del protocolo ofrecido por este
        /// plugin.
        /// </summary>
        public IProtocol<TClient> Protocol { get; } = new TProtocol();

        /// <inheritdoc />
        /// <summary>
        /// Obtiene el tipo del protocolo ofrecido por este plugin.
        /// </summary>
        public Type ProtocolType => typeof(TProtocol);
    }
}