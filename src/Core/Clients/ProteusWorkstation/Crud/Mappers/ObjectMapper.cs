/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Crud.Mappings;
using TheXDS.MCART.Attributes;

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

        public override IPropertyMapping? Map(IPropertyDescription p)
        {
            if (!(p is IObjectPropertyDescription i)) return null;
            return i.Creatable ? (IPropertyMapping)new ObjectEditorMapping(i) : new SearchComboMapping(i);
            //return i.Creatable ? null : new SimpleObjectMapping(p);
        }
    }
}