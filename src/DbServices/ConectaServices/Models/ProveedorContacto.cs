/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Inventario.Models
{
    public class ProveedorContacto : Contact
    {
        public string Cargo { get; set; }
        public virtual Proveedor Proveedor { get; set; }
    }
}
