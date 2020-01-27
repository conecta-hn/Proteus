/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Base;
using Xceed.Wpf.Toolkit;

namespace TheXDS.Proteus.Crud.Mappings
{
    public class LongUpDownMapping : XceedNumericMapping<LongUpDown, long>
    {
        public LongUpDownMapping(IPropertyDescription property) : base(property)
        {
        }
    }
}