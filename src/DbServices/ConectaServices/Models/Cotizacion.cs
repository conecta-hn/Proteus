/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Facturacion.Models.Base;
using TheXDS.Proteus.Models.Base;
using System;
using System.Linq;

namespace TheXDS.Proteus.Facturacion.Models
{
    public class Cotizacion : FacturaBase, IVoidable
    {
        public DateTime? Void { get; set; }

        public static implicit operator Factura(Cotizacion c)
        {
            var f = new Factura
            {
                Cliente = c.Cliente,
                Descuentos = c.Descuentos,
                OtrosCargos = c.OtrosCargos,
                Notas = c.Notas
            };
            f.Items.AddRange(c.Items.Select(p => p.Clone()));

            return f;
        }
    }
}