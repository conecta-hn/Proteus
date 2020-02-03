/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Config;
using TheXDS.MCART.Component;

namespace TheXDS.Proteus.Cmd
{
    public class KickStartArgument : Argument
    {
        public override char? ShortName => 'k';

        public override string Summary => "Inicializa la aplicación por medio de un KickStarter alternativo.";

        public override ValueKind Kind => ValueKind.ValueRequired;

        public override void Run(CmdLineParser args)
        {
            Settings.Default.UseAltLauncher = true;
            Settings.Default.AltLauncher = Value;
        }
    }
}
