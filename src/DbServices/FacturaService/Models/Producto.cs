using TheXDS.Proteus.Models.Base;
using System.Collections.Generic;
using TheXDS.MCART.Types.Base;

namespace TheXDS.Proteus.Models
{
    /// <summary>
    /// Describe un ítem facturable que define un consumo de inventario.
    /// </summary>
    public class Producto : Facturable, IDescriptible
    {
        public string? Description { get; set; }
        public string? Picture { get; set; }
        public int? StockMin { get; set; }
        public int? StockMax { get; set; }
        public int? ExpiryDays { get; set; }
        public virtual List<InventarioKind> Labels { get; set; } = new List<InventarioKind>();
        public virtual List<Batch> Batches { get; set; } = new List<Batch>();
    }
}