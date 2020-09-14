using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheXDS.Proteus.Models
{
    public class SerialBatch : Batch
    {
        public IEnumerable<SerialNum> Available => Serials.Where(p => p.Owner == null && p.Sold == null);

        public override int Qty => Available.Count();

        public virtual List<SerialNum> Serials { get; set; } = new List<SerialNum>();

        public override string RebajarVenta(int qty, Factura f)
        {
            var sb = new StringBuilder();
            while (qty > 0)
            {
                var i = Available.First();
                i.Owner = f.Cliente;
                i.Sold = DateTime.Now;
                sb.AppendLine($"{Item.Name} con número de serie {i.Id}");
                qty--;
            }
            return $"\n{sb}";
        }

        public override Batch Split(int newQty)
        {
            var b = new SerialBatch {
                Item = Item, Lote = Lote
            };
            while (newQty > 0)
            {
                var i = Available.First();
                Serials.Remove(i);
                b.Serials.Add(i);
                newQty--;
            }
            return b;        
        }
    }
}