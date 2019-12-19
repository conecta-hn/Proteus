/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

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