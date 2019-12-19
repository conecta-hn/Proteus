/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using TheXDS.Proteus.Facturacion.Models.Base;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Facturacion.Models
{
    public class Paquete : Facturable, IVoidable
    {
        public virtual List<Facturable> Children { get; set; } = new List<Facturable>();
        public DateTime ValidFrom { get; set; } = DateTime.Now;
        public DateTime? Void { get; set; }
    }
}