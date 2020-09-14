using System;
using System.Collections.Generic;
using System.Linq;
using TheXDS.MCART;
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

        /// <summary>
        /// Crédito pre-pagado del cliente.
        /// </summary>
        public decimal Prepaid { get; set; }

        /// <summary>
        /// Exoneraciones de ISV otorgadas.
        /// </summary>
        public virtual List<IsvExoneracion> Exoneraciones { get; set; } = new List<IsvExoneracion>();
        
        /// <summary>
        /// Colección de créditos del cliente.
        /// </summary>
        public virtual List<FacturaXCobrar> Debits { get; set; } = new List<FacturaXCobrar>();

        public bool AnyExoneraciones()
        {
            return Exoneraciones.Any(p => DateTime.Now.IsBetween(p.Timestamp.Date, p.Void.Date + TimeSpan.FromDays(1)));
        }

        public bool CanCredit { get; set; } = false;

        public decimal? CreditLimit { get; set; } = null;

        public bool CanPrepay { get; set; } = true;

        public decimal TotalCredits => Debits.Sum(p => p.Total);
        public decimal TotalDue => TotalCredits - Debits.Sum(p => p.Abonos.Sum(q => q.Amount));
        public decimal? AvailableCredit => CreditLimit.HasValue ? CreditLimit - TotalDue : null;

    }
}