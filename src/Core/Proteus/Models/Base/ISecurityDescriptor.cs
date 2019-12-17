namespace TheXDS.Proteus.Models.Base
{
    public interface ISecurityDescriptor
    {
        IProteusCredential Parent { get; }
    }
}