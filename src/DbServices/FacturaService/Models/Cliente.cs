using System;
using System.Collections.Generic;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    /// <summary>
    /// Representa a un cliente dentro del sistema.
    /// </summary>
    public class Cliente : Addressable<int>, ITimestamp
    {
        /// <summary>
        /// Fecha de creación.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// RTN de facturación.
        /// </summary>
        public string? Rtn { get; set; }

        /// <summary>
        /// Categoría a la que el cliente pertenece.
        /// </summary>
        public virtual ClienteCategory? Category { get; set; }

        /// <summary>
        /// Facturaciones realizadas al cliente.
        /// </summary>
        public virtual List<Factura> Facturas { get; set; } = new List<Factura>();

        /// <summary>
        /// Colección de cotizaciones solicitadas por el cliente.
        /// </summary>
        public virtual List<Cotizacion> Cotizaciones { get; set; } = new List<Cotizacion>();
    }
}
