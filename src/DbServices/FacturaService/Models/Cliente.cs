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
        public virtual List<FacturaXCobrar> Credits { get; set; } = new List<FacturaXCobrar>();

        public string SagRef { get; set; }

        public bool AnyExoneraciones()
        {
            return Exoneraciones.Any(p => DateTime.Now.IsBetween(p.Timestamp.Date, p.Void.Date + TimeSpan.FromDays(1)));
        }
    }

    public class IsvExoneracion : TimestampModel<string>
    {
        public DateTime Void { get; set; }
    }

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

    public class Abono : TimestampModel<long>
    {
        public virtual FacturaXCobrar Parent { get; set; }
        public decimal Amount { get; set; }
    }
}