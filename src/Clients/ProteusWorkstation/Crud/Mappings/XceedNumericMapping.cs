/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Crud.Mappings.Base;
using Xceed.Wpf.Toolkit;

namespace TheXDS.Proteus.Crud.Mappings
{
    public abstract class XceedNumericMapping<TControl, T> : XceedUpDownMapping<TControl, T> where TControl : CommonNumericUpDown<T>, new() where T : struct, IComparable<T>, IFormattable
    {
        public XceedNumericMapping(IPropertyDescription property) : base(property)
        {
            if (property is IPropertyNumberDescription<int> d)
            {
                Control.FormatString = d.Format ?? string.Empty;
            }
        }
    }
}