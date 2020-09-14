using System;
using System.Collections.Generic;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    /// <summary>
    /// Representa una solicitud de rangos de facturación junto con los rangos
    /// autorizados.
    /// </summary>
    public class Cai : TimestampModel<string>, IVoidable
    {
        /// <summary>
        /// Colección de rangos de facturación autorizados.
        /// </summary>
        public virtual List<CaiRango> Rangos { get; set; } = new List<CaiRango>();

        /// <summary>
        /// Fecha de vencimiento de la solicitud y todos sus rangos.
        /// </summary>
        public DateTime? Void { get; set; }
    }
}
