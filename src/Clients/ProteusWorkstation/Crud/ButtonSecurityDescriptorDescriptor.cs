/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.Crud
{
    public class ButtonSecurityDescriptorDescriptor : SecurityDescriptorDescriptor<ButtonSecurityDescriptor>
    {
        protected override void DescribeModel()
        {
            FriendlyName("Descriptor de botón");
            base.DescribeModel();
            Property(p => p.Visible);
            Property(p => p.Available);
            Property(p => p.Elevatable);
        }
    }
}