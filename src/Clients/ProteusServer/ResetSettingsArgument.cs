/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.MCART.Component;
using TheXDS.Proteus.Config;

namespace TheXDS.Proteus.Cmd
{
    public class ResetSettingsArgument : Argument
    {
        public override string? Summary => "Borra la configuración actual y regresa todas las configuraciones a sus valores predeterminados.";
        public override void Run(CmdLineParser args)
        {
            Settings.Default.Reset();
        }
    }
}