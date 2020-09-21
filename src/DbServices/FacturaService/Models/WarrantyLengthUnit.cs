using TheXDS.MCART.Attributes;

namespace TheXDS.Proteus.Models
{
    public enum WarrantyLengthUnit
    {
        [Name("Días")] Days,
        [Name("Semanas")] Weeks,
        [Name("Meses")] Months,
        [Name("Años")] Years
    }
}