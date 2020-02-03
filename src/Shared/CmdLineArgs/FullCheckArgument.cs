/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Config;
using TheXDS.MCART.Component;

namespace TheXDS.Proteus.Cmd
{
    public class FullCheckArgument : Argument
    {
        public override char? ShortName => 'f';

        public override string Summary => "Indica a Proteus que realizará una inicialización completa de la base de datos. Equivalente a --Init:Full";

        public override void Run(CmdLineParser args)
        {
            Settings.Default.InitMode = Proteus.InitMode.Full;
        }
    }

}