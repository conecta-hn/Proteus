using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    public class WarrantyDefinition : Nameable<int>
    {
        public string? TemplatePath { get; set; }
        public int? WarrantyLength { get; set; }
        public WarrantyLengthUnit? Unit { get; set; }
    }
}