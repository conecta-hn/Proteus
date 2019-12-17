/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Threading;
using TheXDS.MCART.Component;

namespace TheXDS.Proteus.Cmd
{
    public class LowPriorityArgument : Argument
    {
        public override string LongName => "LowPriority";
        public override char? ShortName => 'l';
        public override string Summary => "Establece la prioridad del proceso de esta aplicación en la más baja.";
        public override void Run(CmdLineParser args)
        {
            try
            {
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                var m = "Modo de proceso de baja prioridad.";
                if (Proteus.AlertTarget is null)
                {
                    Proteus.MessageTarget?.Info(m);
                }
                else
                {
                    Proteus.AlertTarget.Alert(m);
                }
            }
            catch (Exception ex)
            {
                Proteus.MessageTarget?.Warning($"No se pudo establecer la prioridad del proceso: {ex.Message}");
            }
        }
    }
}