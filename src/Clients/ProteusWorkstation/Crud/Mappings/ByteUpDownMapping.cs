/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Base;
using Xceed.Wpf.Toolkit;

namespace TheXDS.Proteus.Crud.Mappings
{
    public class ByteUpDownMapping : XceedNumericMapping<ByteUpDown, byte>
    {
        public ByteUpDownMapping(IPropertyDescription property) : base(property)
        {
        }
    }
    public class ByteUpDownRangeMapping : XceedNumericRangeMapping<ByteUpDown, byte>
    {
        public ByteUpDownRangeMapping(IPropertyDescription property) : base(property)
        {
        }
    }
}