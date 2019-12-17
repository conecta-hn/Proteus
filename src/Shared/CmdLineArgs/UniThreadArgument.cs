/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Diagnostics;
using TheXDS.MCART.Component;

namespace TheXDS.Proteus.Cmd
{
    public class UniThreadArgument : Argument
    {
        public override string LongName => "UniThread";
        public override char? ShortName => 'u';
        public override string Summary => "Establece la afinidad de CPU de esta aplicación para utilizar únicamente el último procesador lógico del sistema.";
        public override void Run(CmdLineParser args)
        {
            try
            {
                Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)(1 << (Environment.ProcessorCount - 1));
                var m = $"Modo de afinidad uni-proceso. Utilizando procesador lógico {Environment.ProcessorCount - 1}";
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
                Proteus.MessageTarget?.Warning($"No se pudo establecer la afinidad del proceso: {ex.Message}");
            }
        }
    }
}