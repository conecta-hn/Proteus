/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;

namespace TheXDS.Proteus.Models.Base
{
    /// <summary>
    /// Clase base para los modelos de datos que describen eventos
    /// ocurridos en un instante de tiempo.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TimestampModel<T> : ModelBase<T>, ITimestamp where T : IComparable<T>
    {
        /// <summary>
        /// Obtiene o establece el instante de tiempo relacionado a esta entidad.
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}