/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Config;
using System;
using TheXDS.MCART.Component;

namespace TheXDS.Proteus.Cmd
{
    public class InitArgument : Argument
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
        }
    }

}