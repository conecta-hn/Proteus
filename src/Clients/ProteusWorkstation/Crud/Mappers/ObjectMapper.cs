/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Crud.Mappings;
using TheXDS.MCART.Attributes;
using TheXDS.MCART.ViewModel;

namespace TheXDS.Proteus.Crud.Mappers
{
    [Priority(2)]
    public class ObjectMapper : PropertyMapper
    {
        public override bool Maps(IPropertyDescription property)
        {
            return property is IObjectPropertyDescription 
                && !(property is IListPropertyDescription);
        }

        public override IPropertyMapping? Map(IEntityViewModel parentVm, IPropertyDescription p)
        {
            if (!(p is IObjectPropertyDescription i)) return null;
            return new ObjectEditorMapping(parentVm, i);
        }
    }
}