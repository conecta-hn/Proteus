using TheXDS.Proteus.Api;

namespace TheXDS.Proteus.Models.Base
{
    public interface IServiceSecurityDescriptor
    {
        string Id { get; }
        SecurityFlags Granted { get; }
        SecurityFlags Revoked { get; }
    }
}