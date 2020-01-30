using System;
using System.Collections.Generic;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    /// <summary>
    /// Representa una sesión de caja.
    /// </summary>
    public class CajaOp : TimestampModel<int>
    {
        /// <summary>
        /// Estación de facturación donde se ha abierto la sesión de caja.
        /// </summary>
        public virtual Estacion Estacion { get; set; } = null!;

        /// <summary>
        /// Cajero que ha abierto la sesión de caja.
        /// </summary>
        public virtual Cajero Cajero { get; set; } = null!;

        /// <summary>
        /// Valor con el que la sesión de caja ha sido abierta.
        /// </summary>
        public decimal OpenBalance { get; set; }

        /// <summary>
        /// Marca de tiempo del cierre de la sesión de caja.
        /// </summary>
        /// <value>
        /// Un <see cref="DateTime"/> con la marca de tiempo del cierre de la
        /// sesión de caja, o <see langword="null"/> si la sesión de caja está
        /// abierta.
        /// </value>
        public DateTime? CloseTimestamp { get; set; }

        /// <summary>
        /// Balance de cierre de sesión de caja.
        /// </summary>
        public decimal? CloseBalance { get; set; }

        /// <summary>
        /// Colección de facturas creadas en esta sesión de caja.
        /// </summary>
        public virtual List<Factura> Facturas { get; set; } = new List<Factura>();
    }
}