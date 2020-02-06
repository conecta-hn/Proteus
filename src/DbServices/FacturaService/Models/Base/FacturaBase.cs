using System.Collections.Generic;
using System.Linq;

namespace TheXDS.Proteus.Models.Base
{
    public abstract class FacturaBase : TimestampModel<long>
    {
        public virtual Cliente Cliente { get; set; }
        public decimal Descuentos { get; set; }
        public decimal OtrosCargos { get; set; }
        public string Notas { get; set; }
        public virtual List<ItemFactura> Items { get; set; } = new List<ItemFactura>();
        public decimal SubTotal => Items.Sum(p => p.SubTotal);
        public decimal SubTGravable => Items.Sum(p => p.MontoGravado);
        public decimal SubTGravado => Items.Sum(p => p.SubTGrav);
        public decimal SubTFinal => Items.Sum(p => p.SubTFinal);
        public decimal Total => SubTFinal - Descuentos + OtrosCargos;
    }

}
