/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.MCART.Component;
using TheXDS.Proteus.Component;

namespace TheXDS.Proteus.Cmd
{
    public class LogFileArgument : Argument
    {
        public override ValueKind Kind => ValueKind.Optional;

        public override string Summary => "Indica que se deberá escribir un archivo de bitácora para todos los mensajes de la sesión.";

        public override void Run(CmdLineParser args)
        {
            CompoundTarget.Register(Value is { } ? new TextFileLogger(Value) : new TextFileLogger());
        }
    }

}