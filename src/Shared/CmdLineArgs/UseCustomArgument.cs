/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Config;
using TheXDS.MCART.Component;

namespace TheXDS.Proteus.Cmd
{
    public class UseCustomArgument : ProteusArgument
    {
        public override ValueKind Kind => ValueKind.Optional;

        public override string Summary => "Cambia el método de conexión de Proteus para utilizar la cadena personalizada especificada.";

        public override void Run(CmdLineParser args)
        {
            if (!(Value is null) || string.IsNullOrWhiteSpace(Value))
            {
                Settings.Default.UseLocalDbProvider = false;
                Settings.Default.UseDomainProvider = false;
                Settings.Default.UseCustomProvider = true;
                Settings.Default.CustomProvider = Value;
            }
            else
            {
                InvalidArg();
            }
        }
    }
}