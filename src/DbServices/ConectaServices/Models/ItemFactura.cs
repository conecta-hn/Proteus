/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.Facturacion.Models.Base;
using System;

namespace TheXDS.Proteus.Facturacion.Models
{
    public class ItemFactura : ModelBase<long>
    {
        public virtual Facturable Item { get; set; }
        public virtual FacturaBase Parent { get; set; }
        public int Qty { get; set; }

        /// <summary>
        ///     Almacena de forma estática el precio de venta utilizado para
        ///     este ítem cuando fue fecturado.
        /// </summary>
        public decimal StaticPrecio { get; set; }

        /// <summary>
        ///     Almacena de forma estática el porcentaje de ISV aplicado a este
        ///     ítem cuando fue fecturado.
        /// </summary>
        public float? StaticIsv { get; set; }

        public decimal StaticDescuento { get; set; }

        public decimal SubTotal => Item.Precio * Qty;
        public decimal MontoGravado => SubTotal * (decimal)(StaticIsv ?? 0f);
        public decimal SubTGrav => SubTotal + MontoGravado;
        public decimal SubTFinal => SubTGrav - StaticDescuento;

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
    }
}