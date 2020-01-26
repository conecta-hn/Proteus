/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Config;
using TheXDS.MCART.Component;

namespace TheXDS.Proteus.Component
{
    /// <summary>
    /// Argumento que permite especificar el número de puerto por medio del
    /// cual conectarse a un servidor de arbitraje de sesión.
    /// </summary>
    public class NwServerPortArgument : Argument
    {
        /// <inheritdoc/>
        public override char? ShortName => 'p';

        /// <inheritdoc/>
        public override string Default => 51201.ToString();

        /// <inheritdoc/>
        public override ValueKind Kind => ValueKind.ValueRequired;

        /// <inheritdoc/>
        public override string Summary => "Indica número de puerto del servidor al cual se realizará la conexión.";

        /// <inheritdoc/>
        public override void Run(CmdLineParser args)
        {
            Settings.Default.NetworkServerPort = int.TryParse(Value ?? Default, out var v) ? v : 51201;
        }
    }    
}