/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Crud.Mappings;
using TheXDS.MCART.Attributes;

namespace TheXDS.Proteus.Crud.Mappers
{
    [Priority(1)]
    public class ListMapper : PropertyMapper
    {
        public override bool Maps(IPropertyDescription property)
        {
            return property is IListPropertyDescription;
        }

        public override IPropertyMapping Map(IPropertyDescription p)
        {
            if (!(p is IListPropertyDescription i)) return null;
            return new ListMapping(i);
        }
    }

}