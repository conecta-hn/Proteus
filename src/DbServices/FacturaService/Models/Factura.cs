using System.Collections.Generic;
using System.Linq;
using TheXDS.MCART.Types.Base;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{

    /// <summary>
    /// Ítem de una orden de trabajo.
    /// </summary>
    public class OrdenTrabajoItem : ModelBase<long>
    {
        /// <summary>
        /// Elemento facturable para el cual se ha creado este ítem.
        /// </summary>
        public virtual Facturable Item { get; set; }

        /// <summary>
        /// Orden de trabajo donde existe este ítem.
        /// </summary>
        public virtual OrdenTrabajo Parent { get; set; }

        /// <summary>
        /// Cantidad de ítems a procesar.
        /// </summary>
        public int Qty { get; set; }

        public override string ToString()
        {
            return $"{Qty} {Item?.Name}";
        }
    }

    /// <summary>
    /// Describe una factura en el sistema.
    /// </summary>
    public class Factura : FacturaBase, IDescriptible
    {
        /// <summary>
        /// Orden de trabajo que ha sido pagada con esta factura.
        /// </summary>
        public virtual OrdenTrabajo OtRef { get; set; }

        /// <summary>
        /// Obtiene un código de compra exenta.
        /// </summary>
        public string CompraExentaRef { get; set; }
        
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
        public string Description => FactNum ?? "n/a";

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

        public decimal TotalPagadoEfectivo => Payments.Where(p => p.Source.ToString() == "419d06c5-7a47-44d5-824b-bc573811084a").Sum(p => p.Amount) + Vuelto;

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
}