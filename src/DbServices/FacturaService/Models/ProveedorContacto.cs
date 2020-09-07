using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    public class ProveedorContacto : Contact
    {
        public string Cargo { get; set; }
        public virtual Proveedor Proveedor { get; set; }
    }
}