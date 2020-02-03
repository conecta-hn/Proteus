/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Config;
using TheXDS.MCART.Component;

namespace TheXDS.Proteus.Cmd
{
    public class ConfigureArgument : Argument
    {
        public override char? ShortName => 'c';

        public override string Summary => $"Indica a {App.Info.Name} que ejecute la configuración inicial.";

        public override void Run(CmdLineParser args)
        {
            Settings.Default.Launched = false;
        }
    }
}
