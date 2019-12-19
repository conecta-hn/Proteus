/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Collections.Generic;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Facturacion.Models
{
    public class ClienteCategory : Nameable<int>
    {
        public bool RequireRTN { get; set; }
        public virtual List<Cliente> Members { get; set; } = new List<Cliente>();
        public virtual TablaPrecio AltPrecios { get; set; }
    }
}