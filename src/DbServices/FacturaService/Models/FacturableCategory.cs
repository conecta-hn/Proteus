using System.Collections.Generic;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    public class FacturableCategory : Nameable<int>
    {
        public virtual List<Facturable> Children { get; set; } = new List<Facturable>();
        public float? Isv { get; set; }
    }
}