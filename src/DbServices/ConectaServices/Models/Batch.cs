using System.Collections.Generic;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Inventario.Models
{
    public class Batch : TimestampModel<long>
    {   
        public virtual Bodega Bodega { get; set; }
        public virtual Proveedor Proveedor { get; set; }
        public virtual EquipoDefinition Definition { get; set; }
        public virtual List<EquipoInstance> Items { get; set; } = new List<EquipoInstance>();
        public int Qty => Items.Count;
    }
}