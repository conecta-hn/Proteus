using System;
using System.Collections.Generic;
using System.Linq;
using TheXDS.MCART.Types.Base;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    /// <summary>
    /// Describe una factura en el sistema.
    /// </summary>
    public class Factura : FacturaBase, IDescriptible
    {
        /// <summary>
        /// Correlativo interno dentro del rango de facturación.
        /// </summary>
        public int Correlativo { get; set; }

        /// <summary>
        /// Rango de facturación al cual esta factura pertenece.
        /// </summary>
        public virtual CaiRango CaiRangoParent { get; set; } = null!;

        /// <summary>
        /// Sesión de caja en la cual esta factura ha sido creada.
        /// </summary>
        public virtual CajaOp Parent { get; set; } = null!;

        /// <summary>
        /// Colección de pagos agregados a esta factura.
        /// </summary>
        public virtual List<Payment> Payments { get; set; } = new List<Payment>();

        /// <summary>
        /// Obtiene un valor que indica si esta factura ha sido anulada.
        /// </summary>
        public bool Nula { get; set; }

        /// <summary>
        /// Obtiene un valor que indica si esta factura ya ha sido impresa.
        /// </summary>
        public bool Impresa { get; set; }

        /// <summary>
        /// Obtiene una descripción amigable para la factura.
        /// </summary>
        public string Description => $"{Id}{FactNum?.OrNull(" ({0})")}";

        /// <summary>
        /// Obtiene una cadena que representa el número de facturación.
        /// </summary>
        public string? FactNum => CaiRangoParent?.FactNum(Correlativo);

        /// <summary>
        /// Obtiene un valor que indica el monto cancelado del valor total de
        /// la factura.
        /// </summary>
        public decimal Paid => Payments.Sum(p => p.Amount);

        /// <summary>
        /// Obtiene un valor que indica la cantidad de vuelto a favor del cliente.
        /// </summary>
        public decimal Vuelto => Total - Paid;

        /// <summary>
        /// Convierte una instancia de la clase <see cref="Factura"/> en una
        /// <see cref="Cotizacion"/>
        /// </summary>
        /// <param name="f"></param>
        public static implicit operator Cotizacion(Factura f)
        {
            var c = new Cotizacion
            {
                Cliente = f.Cliente,
                Descuentos = f.Descuentos,
                OtrosCargos = f.OtrosCargos,
                Notas = f.Notas
            };
            c.Items.AddRange(f.Items.Select(p => p.Clone()));
            return c;
        }
    }

    public class Paquete : Facturable, IVoidable
    {
        public virtual List<Facturable> Children { get; set; } = new List<Facturable>();
        public DateTime ValidFrom { get; set; } = DateTime.Now;
        public DateTime? Void { get; set; }
    }

    public class Payment : TimestampModel<long>
    {
        public virtual Factura Parent { get; set; }
        public Guid Source { get; set; }
        public decimal Amount { get; set; }
        public override string ToString()
        {
            return $"{FacturaService.PaymentSources.FirstOrDefault(p => p.Guid == Source)?.Name}: {Amount:C}";
        }
    }

    public class Producto : Facturable
    {
    }
    /// <summary>
    /// Describe un servicio facturable sin consumo de inventario.
    /// </summary>
    public class Servicio : Facturable
    {
    }
    public class TablaPrecio : Nameable<int>
    {
        public virtual List<TablaPrecioItem> Items { get; set; } = new List<TablaPrecioItem>();
        public virtual List<ClienteCategory> AppliedTo { get; set; } = new List<ClienteCategory>();
    }
    public class TablaPrecioItem : Valuable<int>
    {
        public virtual Facturable Item { get; set; }
    }
}