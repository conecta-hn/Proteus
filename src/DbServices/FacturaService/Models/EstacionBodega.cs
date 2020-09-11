using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    public class EstacionBodega : ModelBase<int>
    {
        public virtual Estacion Estacion { get; set; }
        public virtual Bodega Bodega { get; set; }

        public override string ToString()
        {
            return Bodega?.Name ?? Estacion?.Name ?? "Enlace Estación/Bodega sin definir";
        }
    }
}