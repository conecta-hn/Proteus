/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Base;
using Xceed.Wpf.Toolkit;

namespace TheXDS.Proteus.Crud.Mappings
{
    public class DoubleUpDownMapping : XceedNumericMapping<DoubleUpDown, double>
    {
        public DoubleUpDownMapping(IPropertyDescription property) : base(property)
        {
        }
    }
    public class DoubleUpDownRangeMapping : XceedNumericRangeMapping<DoubleUpDown, double>
    {
        public DoubleUpDownRangeMapping(IPropertyDescription property) : base(property)
        {
        }
    }
}