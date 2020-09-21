using System.Collections.Generic;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    public class InventarioKind : Nameable<int>
    {
        public virtual List<InventarioKindItem> Items { get; set; } = new List<InventarioKindItem>();
    }
}