/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Facturacion.Models.Base;
using System.Collections.Generic;
using System.Linq;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Proteus.Facturacion.Models
{
    public class Factura : FacturaBase, IDescriptible
    {
        public int Correlativo { get; set; }
        public virtual CaiRango CaiRangoParent { get; set; }
        public virtual List<Payment> Payments { get; set; } = new List<Payment>();
        public bool Nula { get; set; }
        public bool Impresa { get; set; }

        public string Description => $"{Id}{FactNum?.OrNull(" ({0})")}";
        public string FactNum => CaiRangoParent?.FactNum(Correlativo);
        public decimal Paid => Payments.Sum(p => p.Amount);
        public decimal Vuelto => Total - Paid;

        public static implicit operator Cotizacion(Factura f)
        {
            var c = new Cotizacion
            {
                Cliente = f.Cliente,
                Descuentos = f.Descuentos,
                OtrosCargos = f.OtrosCargos,
                Notas = f.Notas
            };
            c.Items.AddRange(f.Items.Select(p => p.Clone()));
            return c;
        }
    }
}