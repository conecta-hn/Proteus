/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    /// <summary>
    /// Modelo que describe un aviso general.
    /// </summary>
    public class Aviso : TitledText<int>, ITimestamp
    {
        /// <summary>
        /// Obtiene o establece la marca de tiempo de creación de este
        /// aviso.
        /// </summary>
        public DateTime Timestamp { get ; set; }
    }
}