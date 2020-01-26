/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Crud.Mappers.Base;
using TheXDS.Proteus.Crud.Mappings;
using TheXDS.MCART.Types;

namespace TheXDS.Proteus.Crud.Mappers
{
    public sealed class DateMapper : PropertyMapper
    {
        public override IPropertyMapping Map(IPropertyDescription p)
        {
            return new DateTimeUpDownMapping(p);
            if (p is IPropertyDateDescription i)
            {
                return i.WithTime ? new DateTimeUpDownMapping(i) : (IPropertyMapping) new DateMapping(i);
            }
            return null;
        }
        public override bool Maps(IPropertyDescription property)
        {
            var r = property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?);
            return r;
        }
    }
    public sealed class DateTimeRangeMapper : SimpleMapper<Range<DateTime>, DateTimeUpDownRangeMapping> { }
}