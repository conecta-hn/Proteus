using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    public class Actividad : TimestampModel<string>
    {
        public string Description { get; set; }
        public virtual List<ActividadItem> Children { get; set; } = new List<ActividadItem>();

        public decimal UtilidadNeta => Children.Sum(p => p.Movimiento);
        public decimal Entradas => Children.Select(p => p.Movimiento).Where(p => p > 0m).Sum();
        public decimal Gastos => Children.Select(p => p.Movimiento).Where(p => p < 0m).Sum();
    }
    public class ActividadItem : TimestampModel<long>
    {
        public string Description { get; set; }
        public decimal Movimiento { get; set; }
    }
}
