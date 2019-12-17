/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Config;
using TheXDS.MCART.Component;

namespace TheXDS.Proteus.Component
{
    public class NwServerPortArgument : Argument
    {
        public override char? ShortName => 'p';
        public override string Default => 51201.ToString();
        public override ValueKind Kind => ValueKind.ValueRequired;
        public override string Summary => "Indica número de puerto del servidor al cual se realizará la conexión.";
        public override void Run(CmdLineParser args)
        {
            Settings.Default.NetworkServerPort = int.TryParse(Value ?? Default, out var v) ? v : 51201;
        }
    }    
}