/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Protocols;
using TheXDS.MCART.PluginSupport.Legacy;

namespace TheXDS.Proteus.Plugins
{
    /// <summary>
    /// Clase base para un <see cref="Plugin"/> que permite extender al
    /// protocolo de sesión de Proteus.
    /// </summary>
    public abstract class ProteusProtocolExtensionPlugin : Plugin
    {
        /// <summary>
        /// Extiende al protocolo de sesión de Proteus conectando nuevos
        /// comandos y manejadores.
        /// </summary>
        /// <param name="protocol">
        /// Instancia del protocolo a extender.
        /// </param>
        protected internal abstract void WireUp(SessionProtocol protocol);
    }
}