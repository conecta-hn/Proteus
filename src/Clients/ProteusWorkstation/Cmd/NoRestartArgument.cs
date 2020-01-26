/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Config;
using TheXDS.MCART.Component;

namespace TheXDS.Proteus.Cmd
{
    public class NoRestartArgument : Argument
    {
        public override string Summary => $"Intenta aplicar los cambios de configuración sin reiniciar {App.Info.Name} (experimental)";

        public override void Run(CmdLineParser args)
        {
            Settings.Default.ResartRequired = false;
        }
    }
}
