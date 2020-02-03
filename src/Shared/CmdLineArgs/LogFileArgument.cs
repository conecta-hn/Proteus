/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Linq;
using TheXDS.MCART.Component;
using TheXDS.Proteus.Component;

namespace TheXDS.Proteus.Cmd
{
    public class LogFileArgument : Argument
    {
        public TextFileLogger? Logger { get; private set; }
        public override ValueKind Kind => ValueKind.Optional;

        public override string Summary => "Indica que se deberá escribir un archivo de bitácora para todos los mensajes de la sesión.";

        public override void Run(CmdLineParser args)
        {
            if ((Proteus.MessageTarget as CompoundTarget)?.Targets.OfType<TextFileLogger>().Any() ?? false) return;
            Logger = Value is { } ? new TextFileLogger(Value) : new TextFileLogger();
            CompoundTarget.Register(Logger);
            CompoundReporter.Register(Logger);
        }
    }
}