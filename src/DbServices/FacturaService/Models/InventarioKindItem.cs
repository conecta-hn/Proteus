using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    public class InventarioKindItem : ModelBase<long>
    {
        public virtual InventarioKind Kind { get; set; }
        public virtual Producto Producto { get; set; }
    }
}