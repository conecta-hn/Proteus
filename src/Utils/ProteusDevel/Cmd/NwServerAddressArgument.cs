/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Config;
using TheXDS.MCART.Component;

namespace TheXDS.Proteus.Component
{
    /// <summary>
    /// Argumento que permite establecer la dirección del servidor de arbitraje
    /// de sesión al cual coenctarse.
    /// </summary>
    public class NwServerAddressArgument : Argument
    {
        /// <inheritdoc/>
        public override char? ShortName => 's';

        /// <inheritdoc/>
        public override string Default => "localhost";

        /// <inheritdoc/>
        public override ValueKind Kind => ValueKind.ValueRequired;

        /// <inheritdoc/>
        public override string Summary => "Indica el nombre del servidor al cual se realizará la conexión.";

        /// <inheritdoc/>
        public override void Run(CmdLineParser args)
        {
            Settings.Default.NetworkServerAddress = Value ?? Default;
        }
    }
}