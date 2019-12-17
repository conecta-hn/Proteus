/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Config;
using TheXDS.MCART.Component;

namespace TheXDS.Proteus.Cmd
{
    public class UseCustomArgument : Argument
    {
        public override ValueKind Kind => ValueKind.Optional;

        public override string Summary => "Cambia el método de conexión de Proteus para utilizar la cadena personalizada especificada.";

        public override void Run(CmdLineParser args)
        {
            Settings.Default.UseLocalDbProvider = false;
            Settings.Default.UseDomainProvider = false;
            Settings.Default.UseCustomProvider = true;
            if (!(Value is null) || string.IsNullOrWhiteSpace(Value))
            {
                Settings.Default.CustomProvider = Value;
            }
        }
    }
}