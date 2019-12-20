using System.Collections.Generic;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Conecta.Models
{
    public interface IPagable : ITimestamp
    {
        List<Pago> Pagos { get; set; }
        decimal Total { get; set; }
    }
}