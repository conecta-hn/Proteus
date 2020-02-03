/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.MCART.Component;
using TheXDS.Proteus.Config;

namespace TheXDS.Proteus.Cmd
{
    public class ForceNoUserArgument : AdministrativeArgument
    {

        public override string Summary => $"Obliga a {App.Info.Name} a iniciar sin soporte de inicio de sesión.";
        protected override void OnRun(CmdLineParser args)
        {
            LaunchConfig._interactiveLogin = false;
        }
    }
}