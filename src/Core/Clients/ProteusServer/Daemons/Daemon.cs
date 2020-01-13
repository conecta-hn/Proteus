/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using TheXDS.MCART.PluginSupport.Legacy;

namespace TheXDS.Proteus.Daemons
{
    public abstract class Daemon : Plugin, IDaemon
    {
        public virtual bool CanRun => true;

        /// <summary>
        /// Hora de ejecución, en periodos de 15 minutos.
        /// </summary>
        public virtual byte Schedule => (byte)(DateTime.Now.TimeOfDay.TotalMinutes / 15);

        public TimeSpan ScheduleTime => TimeSpan.FromMinutes(Schedule * 15);

        public abstract void Run();
    }
}
