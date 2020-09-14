using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    public class Entidad : Addressable<string>
    {
        public string? Banner { get; set; }
    }
}