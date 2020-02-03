/*
Copyright © 2017-2020 César Andrés Morgan
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
}