using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    /// <summary>
    /// Representa a un ítem dentro de una factura o una cotización.
    /// </summary>
    public class ItemFactura : ModelBase<long>
    {
        /// <summary>
        /// Referencia al ítem facturable.
        /// </summary>
        public virtual Facturable Item { get; set; } = null!;

        /// <summary>
        /// Factura/cotización donde ha sido creado este ítem.
        /// </summary>
        public virtual FacturaBase Parent { get; set; } = null!;

        /// <summary>
        /// Cantidad de ítems a facturar.
        /// </summary>
        public int Qty { get; set; }

        /// <summary>
        /// Almacena de forma estática el precio de venta utilizado para este
        /// ítem cuando fue fecturado.
        /// </summary>
        public decimal StaticPrecio { get; set; }

        /// <summary>
        /// Almacena de forma estática el porcentaje de ISV aplicado a este
        /// ítem cuando fue fecturado.
        /// </summary>
        public float? StaticIsv { get; set; }

        /// <summary>
        /// Almacena de forma estática el descuento de venta utilizado para
        /// este ítem cuando fue fecturado.
        /// </summary>
        public decimal StaticDescuento { get; set; }

        /// <summary>
        /// Obtiene el subtotal simple de este ítem.
        /// </summary>
        public decimal SubTotal => Item.Precio * Qty;

        /// <summary>
        /// Obtiene el montro gravado de este ítem.
        /// </summary>
        public decimal MontoGravado => SubTotal * (decimal)((StaticIsv / 100f) ?? 0f);

        /// <summary>
        /// Obtiene la suma del subtotal y el monto gravado para este ítem.
        /// </summary>
        public decimal SubTGrav => SubTotal + MontoGravado;

        /// <summary>
        /// Obtiene el subtotal final, que incluye la rebaja del descuento
        /// aplicado a este ítem.
        /// </summary>
        public decimal SubTFinal => SubTGrav - StaticDescuento;

        /// <summary>
        /// Crea una nueva copia de esta instancia.
        /// </summary>
        /// <returns>Una nueva copia de esta instancia.</returns>
        public ItemFactura Clone()
        {
            return new ItemFactura
            {
                Item = Item,
                Qty = Qty,
                StaticPrecio = StaticPrecio,
                StaticIsv = StaticIsv,
                StaticDescuento = StaticDescuento
            };
        }

        public override string ToString()
        {
            return $"{Qty}x {Item?.Name ?? "ítems"}";
        }
    }
}