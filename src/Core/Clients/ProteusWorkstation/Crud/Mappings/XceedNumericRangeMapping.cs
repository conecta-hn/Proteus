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
    public abstract class XceedNumericRangeMapping<TControl, T> : XceedUpDownRangeMapping<TControl, T> where TControl : CommonNumericUpDown<T>, new() where T : struct, IComparable<T>, IFormattable
    {
        public XceedNumericRangeMapping(IPropertyDescription property) : base(property)
        {
            if (property is IPropertyNumberDescription<int> d)
            {
                _lower.FormatString = d.Format ?? string.Empty;
                _upper.FormatString = d.Format ?? string.Empty;
            }
        }
    }
}