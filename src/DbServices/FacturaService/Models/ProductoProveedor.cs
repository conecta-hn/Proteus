using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    public class ProductoProveedor: ModelBase<long>
    {
        public virtual Proveedor Proveedor { get; set; }
        public virtual Producto Producto { get; set; }
    }
}