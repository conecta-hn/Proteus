using TheXDS.Proteus.Models.Base;
using System.Collections.Generic;

namespace TheXDS.Proteus.Models
{
    /// <summary>
    /// Describe un ítem facturable que define un consumo de inventario.
    /// </summary>
    public class Producto : Facturable
    {
        public string? Description { get; set; }
        public string? Picture { get; set; }
        public int? StockMin { get; set; }
        public int? StockMax { get; set; }
        public int? ExpiryDays { get; set; }
        public virtual List<InventarioKind> Labels { get; set; } = new List<InventarioKind>();
    }
}