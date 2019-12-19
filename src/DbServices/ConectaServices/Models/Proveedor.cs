/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models.Base;
using System.Collections.Generic;

namespace TheXDS.Proteus.Inventario.Models
{
    public class Proveedor : Addressable
    {
        public string WebPage { get; set; }
        public virtual List<ProveedorContacto> Contactos { get; set; } = new List<ProveedorContacto>();
        public virtual List<EquipoDefinition> Productos { get; set; } = new List<EquipoDefinition>();
    }
}
