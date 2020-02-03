/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using TheXDS.MCART.PluginSupport.Legacy;

namespace TheXDS.Proteus.Daemons
{
    /// <summary>
    /// Clase base para un objeto que describa acciones que pueden ejecutarse
    /// desde el servidor de forma periódica.
    /// </summary>
    public abstract class Daemon : Plugin, IDaemon
    {
        /// <summary>
        /// Obtiene un valor que indica si este <see cref="Daemon"/> puede ser
        /// ejecutado.
        /// </summary>
        public virtual bool CanRun => true;

        /// <summary>
        /// Hora de ejecución, en periodos de 15 minutos.
        /// </summary>
        public virtual byte Schedule => (byte)(DateTime.Now.TimeOfDay.TotalMinutes / 15);

        /// <summary>
        /// Obtiene una programación de tiempo en la cual ejecutar el Daemon.
        /// </summary>
        public TimeSpan ScheduleTime => TimeSpan.FromMinutes(Schedule * 15);

        /// <summary>
        /// Ejecuta el <see cref="Daemon"/>.
        /// </summary>
        public abstract void Run();
    }
}
