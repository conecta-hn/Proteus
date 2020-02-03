/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Crud.Mappings.Base;
using Xceed.Wpf.Toolkit;

namespace TheXDS.Proteus.Crud.Mappings
{
    public class IntUpDownRangeMapping : XceedUpDownRangeMapping<IntegerUpDown, int>
    {
        public IntUpDownRangeMapping(IPropertyDescription property) : base(property)
        {
        }
    }
}