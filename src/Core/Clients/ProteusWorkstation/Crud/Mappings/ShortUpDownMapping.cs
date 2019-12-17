/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Base;
using Xceed.Wpf.Toolkit;

namespace TheXDS.Proteus.Crud.Mappings
{
    public class ShortUpDownMapping : XceedNumericMapping<ShortUpDown, short>
    {
        public ShortUpDownMapping(IPropertyDescription property) : base(property)
        {
        }
    }
    public class ShortUpDownRangeMapping : XceedNumericRangeMapping<ShortUpDown, short>
    {
        public ShortUpDownRangeMapping(IPropertyDescription property) : base(property)
        {
        }
    }
}