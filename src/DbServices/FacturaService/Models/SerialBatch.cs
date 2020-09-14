using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheXDS.Proteus.Models
{
    public class SerialBatch : Batch
    {
        public IEnumerable<SerialNum> Available => Serials.Where(p => p.Sold == null);

        public override int Qty => Available.Count();

        public virtual List<SerialNum> Serials { get; set; } = new List<SerialNum>();

        public override string RebajarVenta(int qty, Factura f)
        {
            var w = Item.Warranty is null ? null : new Warranty
            {
                Cliente = f.Cliente,
                Timestamp = f.Timestamp,
                Void = CalcFrom()
            };
            var sb = new StringBuilder();
            while (qty > 0)
            {
                var i = Available.First();
                i.Warranty = w;
                i.Sold = f.Timestamp;
                sb.AppendLine($"{Item.Name} con número de serie {i.Id}");
                qty--;
            }
            return $"\n{sb}";
        }

        private DateTime? CalcFrom()
        {
            if (!(Item.Warranty is { WarrantyLength: { } i, Unit: { } u })) return null;
            return u switch
            {
                WarrantyLengthUnit.Days => DateTime.Today.AddDays(i),
                WarrantyLengthUnit.Weeks => DateTime.Today.AddDays(i * 7),
                WarrantyLengthUnit.Months => DateTime.Today.AddMonths(i),
                WarrantyLengthUnit.Years => DateTime.Today.AddYears(i),
                _ => null
            };
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