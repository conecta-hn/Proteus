/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Config;
using TheXDS.MCART.Component;

namespace TheXDS.Proteus.Component
{
    public class NwServerAddressArgument : Argument
    {
        public override char? ShortName => 's';
        public override string Default => "localhost";
        public override ValueKind Kind => ValueKind.ValueRequired;
        public override string Summary => "Indica el nombre del servidor al cual se realizará la conexión.";
        public override void Run(CmdLineParser args)
        {
            Settings.Default.NetworkServerAddress = Value ?? Default;
        }
    }
}