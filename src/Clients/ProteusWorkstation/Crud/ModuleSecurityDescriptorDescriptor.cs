/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.Crud
{
    public class ModuleSecurityDescriptorDescriptor : SecurityDescriptorDescriptor<ModuleSecurityDescriptor>
    {
        protected override void DescribeModel()
        {
            FriendlyName("Descriptor de módulo");
            base.DescribeModel();
            Property(p => p.Loaded);
            Property(p => p.Accesible);
        }
    }
}