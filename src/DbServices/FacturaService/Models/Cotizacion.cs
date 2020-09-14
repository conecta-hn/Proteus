using System;
using System.Linq;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    /// <summary>
    /// Representa una cotización dentro del sistema.
    /// </summary>
    public class Cotizacion : FacturaBase, IVoidable
    {
        /// <summary>
        /// Indica la fecha de vencimiento de la cotización.
        /// </summary>
        public DateTime? Void { get; set; }

        /// <summary>
        /// Convierte esta instancia de <see cref="Cotizacion"/> a una de
        /// <see cref="Factura"/> copiando todos los campos relevantes.
        /// </summary>
        /// <param name="c">Instancia a convertir.</param>
        public static implicit operator Factura(Cotizacion c)
        {
            var f = new Factura
            {
                Cliente = c.Cliente,
                Descuentos = c.Descuentos,
                OtrosCargos = c.OtrosCargos,
                Notas = c.Notas
            };
            f.Items.AddRange(c.Items.Select(p => p.Clone()));

            return f;
        }
    }
}