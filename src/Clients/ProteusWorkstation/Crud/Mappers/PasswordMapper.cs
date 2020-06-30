/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Crud.Mappings;

namespace TheXDS.Proteus.Crud.Mappers
{
    public class PasswordMapper : PropertyMapper
    {
        public override bool Maps(IPropertyDescription property)
        {
            var p = property.Property.PropertyType;
            return p == typeof(byte[]);
        }

        public override IPropertyMapping Map(IEntityViewModel parentVm, IPropertyDescription p)
        {
            return new PasswordMapping(p);
        }
    }
}