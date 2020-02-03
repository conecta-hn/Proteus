/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using TheXDS.MCART.Component;

namespace TheXDS.Proteus.Cmd
{
    public class ChDirArgument : Argument
    {
        public override string LongName => "ChDir";
        public override ValueKind Kind => ValueKind.ValueRequired;
        public override string Summary => "Cambia de directorio antes de ejecutar la aplicación.";
        public override void Run(CmdLineParser args)
        {
            Environment.CurrentDirectory = Value;
        }
    }
}