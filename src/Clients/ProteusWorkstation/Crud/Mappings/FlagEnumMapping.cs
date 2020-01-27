/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Crud.Mappings.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using TheXDS.MCART.Types.Extensions;
using TheXDS.MCART;
using static TheXDS.MCART.Types.Extensions.NamedObjectExtensions;

namespace TheXDS.Proteus.Crud.Mappings
{
    public class FlagEnumMapping : PropertyMapping
    {
        private readonly IEnumerable<ToggleButton> _boxes;
        private static FrameworkElement Generate(IPropertyDescription p, out ICollection<ToggleButton> boxes)
        {
            var bxs = new HashSet<ToggleButton>();
            var pnl = new WrapPanel
            {
                Orientation = Orientation.Vertical,
                MaxHeight = 150
            };
            var b = new GroupBox
            {
                Header = p.Label,
                Content = new ScrollViewer
                {
                    Content = pnl,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Auto
                }
            };

            foreach (var j in p.Property.PropertyType.AsNamedEnum())
            {
                if (j.Value.ToUnderlyingType().Equals(0)) continue;
                var chk = new CheckBox { Content = j.Name, Tag = j.Value };
                chk.Click += (sender, e) =>
                {
                    if (sender != e.OriginalSource) return;
                    var v = (Enum)chk.Tag;
                    bool[] bits;
                    switch (v.ToUnderlyingType())
                    {
                        case byte @byte:
                            bits = @byte.ToBits().ToArray();
                            break;
                        case short @short:
                            bits = @short.ToBits().ToArray();
                            break;
                        case int @int:
                            bits = @int.ToBits().ToArray();
                            break;
                        case long @long:
                            bits = @long.ToBits().ToArray();
                            break;
                        default: throw new InvalidOperationException();
                    }

                    if (bits.Count(q => q) > 1)
                    {
                        foreach (var l in bxs)
                        {
                            if (l == chk) continue;
                            if (v.HasFlag((Enum)l.Tag))
                            {
                                l.IsChecked = chk.IsChecked;
                            }
                        }
                    }
                    else
                    {
                        foreach (var l in bxs)
                        {
                            if (l == chk) continue;
                            if (((Enum)l.Tag).HasFlag(v))
                            {
                                l.IsChecked = false;
                            }
                        }
                    }
                };
                pnl.Children.Add(chk);
                bxs.Add(chk);
            }
            boxes = bxs;
            return b;
        }

        public FlagEnumMapping(IPropertyDescription property) : base(property, Generate(property, out var boxes))
        {
            _boxes = boxes;
        }

        public override object ControlValue
        {
            get
            {
                return OrEnumValues();
            }
            set
            {
                foreach (var j in _boxes)
                    j.IsChecked = ((Enum)value).HasFlag((Enum)j.Tag);
            }
        }

        public override void ClearControlValue()
        {
            foreach (var j in _boxes)
                j.IsChecked = false;
        }
        private object OrEnumValues()
        {

            if (Description.Property.PropertyType.GetEnumUnderlyingType() == typeof(byte))
            {
                byte t = 0;
                foreach (var j in _boxes) t |= (j.IsChecked ?? false) ? (byte)j.Tag : (byte)0;
                return t;
            }
            if (Description.Property.PropertyType.GetEnumUnderlyingType() == typeof(short))
            {
                short t = 0;
                foreach (var j in _boxes) t |= (j.IsChecked ?? false) ? (short)j.Tag : (short)0;
                return t;
            }
            if (Description.Property.PropertyType.GetEnumUnderlyingType() == typeof(int))
            {
                var t = 0;
                foreach(var j in _boxes) t |= (j.IsChecked ?? false) ? (int)j.Tag : 0;                
                return t;
            }
            if (Description.Property.PropertyType.GetEnumUnderlyingType() == typeof(long))
            {
                long t = 0;
                foreach (var j in _boxes) t |= (j.IsChecked ?? false) ? (long)j.Tag : 0L;
                return t;
            }
            throw new PlatformNotSupportedException();
        }
    }
}