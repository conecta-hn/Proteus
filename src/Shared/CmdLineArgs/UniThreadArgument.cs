/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Diagnostics;
using TheXDS.MCART.Component;

namespace TheXDS.Proteus.Cmd
{

    public class UniThreadArgument : ProteusArgument
    {
        public override string LongName => "UniThread";
        public override char? ShortName => 'u';
        public override string Summary => "Establece la afinidad de CPU de esta aplicación para utilizar únicamente un procesador lógico del sistema.";
        public override ValueKind Kind => ValueKind.Optional;
        public override void Run(CmdLineParser args)
        {
            try
            {
                if (!int.TryParse(Value ?? "0", out var cpu) || cpu >= Environment.ProcessorCount)
                {
                    InvalidArg();
                    return;
                }
                Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)(1 << cpu);
                Show(p => p.Info, "Modo de afinidad uni-proceso", $"Se ha iniciado la aplicación en modo de afinidad uni-proceso. Utilizando procesador lógico {cpu}");
            }
            catch (Exception ex)
            {
                ShowWarning(ex);
            }
        }
    }
}