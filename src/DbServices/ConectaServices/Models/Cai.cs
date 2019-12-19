/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models.Base;
using System;
using System.Collections.Generic;

namespace TheXDS.Proteus.Facturacion.Models
{
    public class Cai : TimestampModel<string>, IVoidable
    {
        public virtual List<CaiRango> Rangos { get; set; } = new List<CaiRango>();
        public DateTime? Void { get; set; }
    }
}