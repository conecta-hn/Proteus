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
    [Priority(0)]
    public class LinkMapper : PropertyMapper
    {
        public override bool Maps(IPropertyDescription property)
        {
            return property is ILinkPropertyDescription;
        }

        public override IPropertyMapping Map(IEntityViewModel parentVm, IPropertyDescription p)
        {
            if (!(p is ILinkPropertyDescription i)) return null;
            return i.Creatable ? new CreatableLinkMapping(p) : (IPropertyMapping)new SimpleLinkMapping(p);
        }
    }

}