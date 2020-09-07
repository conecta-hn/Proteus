using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    public abstract class Batch : TimestampModel<long>
    {
        public virtual Bodega Bodega { get; set; }
        public virtual Producto Item { get; set; }
        public virtual Lote Lote { get; set; }
        public decimal Costo { get; set; }
        public decimal ItemCosto => Qty > 0 ? Costo / Qty : Costo;
        public abstract int Qty { get; }

        public abstract string RebajarVenta(int qty, Factura f);
        public abstract Batch Split(int newQty);
        public override string ToString()
        {
            return $"{Qty} unidades de {Item.Name}";
        }
    }
}