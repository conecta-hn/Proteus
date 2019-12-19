/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Facturacion.Models
{
    public class Cliente : Contact<int>, ITimestamp
    {
        public DateTime Timestamp { get; set; }
        public string Rtn { get; set; }
        public virtual ClienteCategory Category { get; set; }
        public virtual List<Factura> Facturas { get; set; } = new List<Factura>();
        public virtual List<Cotizacion> Cotizaciones { get; set; } = new List<Cotizacion>();
    }
}