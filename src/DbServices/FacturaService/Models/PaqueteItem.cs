using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    public class PaqueteItem : ModelBase<long>
    {
        public virtual Facturable Item { get; set; }
        public virtual Paquete Parent { get; set; }
    }
}