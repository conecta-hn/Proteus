using TheXDS.Proteus.Models.Base;
using System.Collections.Generic;

namespace TheXDS.Proteus.Models
{
    public class Bodega : Nameable<int>
    {
        public virtual List<Batch> Batches { get; set; } = new List<Batch>();
        public virtual List<EstacionBodega> Estaciones { get; set; } = new List<EstacionBodega>();

    }
}