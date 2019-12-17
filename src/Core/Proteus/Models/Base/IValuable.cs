namespace TheXDS.Proteus.Models.Base
{
    public interface IValuable
    {
        float? PercentValue { get; set; }
        decimal? StaticValue { get; set; }
    }
}