using System;
using System.Linq;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Plugins;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    /// <summary>
    /// Describe un pago realizado a una factura.
    /// </summary>
    public class Payment : TimestampModel<long>
    {
        /// <summary>
        /// Factura en la cual se ha definido este pago.
        /// </summary>
        public virtual Factura Parent { get; set; } = null!;

        /// <summary>
        /// Referencia al <see cref="Guid"/> del <see cref="PaymentSource"/>
        /// utilizado para crear este pago.
        /// </summary>
        public Guid Source { get; set; }

        /// <summary>
        /// Monto pagado representado por este objeto.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// convierte esta instancia a su representación como una cadena.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{FacturaService.PaymentSources.FirstOrDefault(p => p.Guid == Source)?.Name}: {Amount:C}";
        }
    }
}