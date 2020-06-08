using TheXDS.MCART.Attributes;

namespace TheXDS.Proteus.Models
{
    public enum IsvExoApplyRule: byte
    {
        [Name("Aplicar exoneración a todos los productos")]All,
        [Name("Aplicar exoneración a todos los productos, excepto...")] AllBut,
        [Name("Aplicar exoneración únicamente a...")] Only
    }
}