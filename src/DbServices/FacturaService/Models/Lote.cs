using TheXDS.Proteus.Models.Base;
using System.Collections.Generic;
using System;

namespace TheXDS.Proteus.Models
{
    public class Lote : ModelBase<string>
    {
        public DateTime Manufactured { get; set; }
        public virtual List<Batch> OnBatches { get; set; } = new List<Batch>();
    }
}