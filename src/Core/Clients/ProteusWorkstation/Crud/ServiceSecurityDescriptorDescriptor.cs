/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.Crud
{
    public class ServiceSecurityDescriptorDescriptor : SecurityDescriptorDescriptor<ServiceSecurityDescriptor>
    {
        protected override void DescribeModel()
        {
            FriendlyName("Descriptor de servicio");
            base.DescribeModel();
            Property(p => p.Granted);
            Property(p => p.Revoked);
        }
    }
}