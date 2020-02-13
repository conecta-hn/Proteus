/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using TheXDS.Proteus.Crud.Base;
using TheXDS.MCART;
using TheXDS.Proteus.Crud.Mappings;
using TheXDS.MCART.ViewModel;

namespace TheXDS.Proteus.Crud.Mappers
{
    public class EnumMapper : PropertyMapper
    {
        public override bool Maps(IPropertyDescription property)
        {
            var p = property.Property.PropertyType;
            return p.IsEnum || (Nullable.GetUnderlyingType(p)?.IsEnum ?? false);
        }

        public override IPropertyMapping Map(IEntityViewModel parentVm, IPropertyDescription p)
        {
            if (p.Property.PropertyType.HasAttr<FlagsAttribute>()) return new FlagEnumMapping(p);
            return new EnumMapping(p);
        }
    }
}