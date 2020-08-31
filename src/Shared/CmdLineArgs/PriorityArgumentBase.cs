/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Threading;
using TheXDS.MCART.Component;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.Cmd
{
    public abstract class PriorityArgumentBase : ProteusArgument
    {
        private readonly string _label;
        private readonly ThreadPriority _priority;

        protected PriorityArgumentBase(string label, ThreadPriority priority)
        {
            _label = label;
            _priority = priority;
        }

        public override string Summary => $"Establece la prioridad del proceso de esta aplicación como prioridad {_label}.";
        public override void Run(CmdLineParser args)
        {
            try
            {
                Thread.CurrentThread.Priority = _priority;
                Show(p => p.Info, $"Modo de proceso de {_label} prioridad.", $"Se ha iniciado la aplicación en modo de proceso de {_label} prioridad.", SetNormalPrio);
            }
            catch (Exception ex)
            {
                ShowWarning(ex);
            }
        }

        private void SetNormalPrio(Alerta a)
        {
            Thread.CurrentThread.Priority = ThreadPriority.Normal;
            a.Dismiss();
        }
    }
}