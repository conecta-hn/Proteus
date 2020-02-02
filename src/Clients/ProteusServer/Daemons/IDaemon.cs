/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;

namespace TheXDS.Proteus.Daemons
{
    /// <summary>
    /// Define una serie de miembros a implementar por un tipo que permita
    /// ejecutar acciones desde un servidor de Proteus de forma periódica.
    /// </summary>
    public interface IDaemon
    {
        /// <summary>
        /// Obtiene un valor que indica si este <see cref="IDaemon"/> puede ser
        /// ejecutado.
        /// </summary>
        bool CanRun => true;

        /// <summary>
        /// Hora de ejecución, en periodos de 15 minutos.
        /// </summary>
        byte Schedule { get; }

        /// <summary>
        /// Obtiene una programación de tiempo en la cual ejecutar el 
        /// <see cref="IDaemon"/>.
        /// </summary>
        TimeSpan ScheduleTime { get; }

        /// <summary>
        /// Ejecuta el <see cref="IDaemon"/>.
        /// </summary>
        void Run();
    }
}