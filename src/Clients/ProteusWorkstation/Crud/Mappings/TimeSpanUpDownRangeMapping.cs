/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Crud.Mappings.Base;
using Xceed.Wpf.Toolkit;

namespace TheXDS.Proteus.Crud.Mappings
{
    public class TimeSpanUpDownRangeMapping : XceedUpDownRangeMapping<TimeSpanUpDown, TimeSpan>
    {
        public TimeSpanUpDownRangeMapping(IPropertyDescription property) : base(property)
        {
        }
    }
}