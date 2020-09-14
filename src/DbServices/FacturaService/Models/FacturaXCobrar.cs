using System.Linq;
using System.Collections.Generic;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    public class FacturaXCobrar : TimestampModel<int>
    {
        public virtual Cliente Cliente { get; set; }
        public virtual Factura Parent { get; set; }
        public decimal Total { get; set; }
        public virtual List<Abono> Abonos { get; set; } = new List<Abono>();
        public decimal Paid => Abonos.Sum(p => p.Amount);
        public decimal Pending => Total - Paid;

        /// <summary>
        /// Obtiene o establece un valor que indica si esta cuenta ya fue pagada.
        /// </summary>
        /// <remarks>
        /// Se implementa como campo de datos con el propósito de permitir
        /// realizar queries sin incurrir en la generación de una cadena de
        /// consulta demasiado compleja, o evitar limitaciones de Linq.
        /// </remarks>
        public bool IsPaid { get; set; }
    }
}