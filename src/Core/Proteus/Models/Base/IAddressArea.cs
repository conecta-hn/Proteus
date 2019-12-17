namespace TheXDS.Proteus.Models.Base
{
    public interface IAddressArea
    {
        string City { get; set; }
        string Country { get; set; }
        string Province { get; set; }
        string Zip { get; set; }
    }
}