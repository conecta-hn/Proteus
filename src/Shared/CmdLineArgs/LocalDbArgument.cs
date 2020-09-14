/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.MCART.Component;
using TheXDS.Proteus.Config;

namespace TheXDS.Proteus.Cmd
{
    public class LocalDbArgument : Argument
    {
        public override string Summary => "Obliga a Proteus a utilizar un servidor LocalDB";
        public override void Run(CmdLineParser args)
        {
            Settings.Default.UseLocalDbProvider = true;
            Settings.Default.UseDomainProvider = false;
            Settings.Default.UseCustomProvider = false;
        }
    }
}