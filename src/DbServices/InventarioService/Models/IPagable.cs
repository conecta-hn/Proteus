using System.Collections.Generic;
using System.Linq;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Conecta.Models
{
    public interface IPagable : ITimestamp
    {
        List<Pago> Pagos { get; set; }
        decimal Total { get; set; }
        decimal Pagado => Pagos.Any() ? Pagos.Sum(p => p.Abono) : 0m;
        decimal Debe => Total - Pagado;

    }
}