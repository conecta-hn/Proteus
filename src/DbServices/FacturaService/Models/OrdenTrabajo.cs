using System;
using System.Collections.Generic;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    /// <summary>
    /// Representa un listado de ítems a procesar.
    /// </summary>
    public class OrdenTrabajo : TimestampModel<long>
    {
        /// <summary>
        /// Facturas enlazadas.
        /// </summary>
        public virtual List<Factura> Facturas { get; set; } = new List<Factura>();

        /// <summary>
        /// Cliente para el cual se ha creado una orden de trabajo.
        /// </summary>
        public virtual Cliente Cliente { get; set; }

        /// <summary>
        /// Notas personalizadas de la orden de trabajo.
        /// </summary>
        public virtual string Notas { get; set; }
        public DateTime Entrega { get; set; }
        public virtual List<OrdenTrabajoItem> Items { get; set; } = new List<OrdenTrabajoItem>();

        public bool Facturado { get; set; }
    }
}