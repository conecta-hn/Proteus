/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using TheXDS.MCART.Component;
using TheXDS.Proteus.Config;

namespace TheXDS.Proteus.Cmd
{
    public class InitArgument : ProteusArgument
    {
        public override ValueKind Kind => ValueKind.ValueRequired;

        public override char? ShortName => 'i';

        public override string Summary => "Cambia el modo de inicialización de Proteus.";

        public override void Run(CmdLineParser args)
        {
            if (Enum.TryParse(Value, out Proteus.InitMode mode))
            {
                Settings.Default.InitMode = mode;
            }
            else
            {
                InvalidArg();
            }
        }
    }
}