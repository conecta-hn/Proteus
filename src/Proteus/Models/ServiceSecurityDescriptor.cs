/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Api;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    public class ServiceSecurityDescriptor : SecurityDescriptor, IServiceSecurityDescriptor
    {
        public SecurityFlags Granted { get; set; }
        public SecurityFlags Revoked { get; set; }
    }
}
