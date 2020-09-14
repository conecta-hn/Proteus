using TheXDS.Proteus.Models.Base;
using System.Collections.Generic;

namespace TheXDS.Proteus.Models
{
    public class Proveedor : Addressable
    {
        public string? WebPage { get; set; }
        public virtual List<ProveedorContacto> Contactos { get; set; } = new List<ProveedorContacto>();
        public virtual List<Producto> Productos { get; set; } = new List<Producto>();
    }
}