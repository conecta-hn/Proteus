namespace TheXDS.Proteus.Models.Base
{
    public interface IAddressable : IAddressArea
    {
        string Address { get; set; }
    }
}