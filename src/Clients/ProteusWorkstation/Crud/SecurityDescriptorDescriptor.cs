/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Crud
{
    public abstract class SecurityDescriptorDescriptor<T> : CrudDescriptor<T> where T : SecurityDescriptor, new()
    {
        protected override void DescribeModel()
        {
            Property(p => p.Id).Id("API objetivo");
        }
    }
}