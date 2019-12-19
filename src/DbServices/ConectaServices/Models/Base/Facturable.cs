/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TheXDS.Proteus.Facturacion.Models.Base
{
    public abstract class Facturable : Nameable<int>
    {
        public float? Isv { get; set; }
        public decimal Precio { get; set; }
        public virtual List<ItemFactura> Instances { get; set; } = new List<ItemFactura>();
    }
}
