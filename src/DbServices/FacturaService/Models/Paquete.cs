using System;
using System.Collections.Generic;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    /// <summary>
    /// Describe un ítem facturable especial que contiene un conjunto de ítems
    /// facturables, como promociones o combos.
    /// </summary>
    public class Paquete : Facturable, IVoidable
    {
        /// <summary>
        /// Colección de ítems a incluir en la facturación al seleccionar este
        /// ítem facturable.
        /// </summary>
        public virtual List<Facturable> Children { get; set; } = new List<Facturable>();

        /// <summary>
        /// Indica una fecha inicial de validez del ítem. De forma
        /// predeterminada, se establece en la fecha de creación del ítem.
        /// </summary>
        public DateTime ValidFrom { get; set; } = DateTime.Now;

        /// <summary>
        /// Fecha opcional de vencimiento del ítem. Aplicable a promociones.
        /// </summary>
        public DateTime? Void { get; set; }
    }
}