using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    public class Abono : TimestampModel<long>
    {
        public virtual FacturaXCobrar Parent { get; set; }
        public decimal Amount { get; set; }
    }
}