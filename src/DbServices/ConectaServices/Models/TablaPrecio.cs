/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Collections.Generic;
using TheXDS.Proteus.Models.Base;
using TheXDS.MCART.Types;

namespace TheXDS.Proteus.Facturacion.Models
{
    public class TablaPrecio : Nameable<int>
    {
        public float? GlobalDescuento { get; set; }
        public virtual List<TablaPrecioItem> Items { get; set; } = new List<TablaPrecioItem>();
        public virtual List<ClienteCategory> AppliedTo { get; set; } = new List<ClienteCategory>();
    }
}
