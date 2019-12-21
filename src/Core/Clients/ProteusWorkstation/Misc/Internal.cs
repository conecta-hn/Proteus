/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using TheXDS.MCART;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.Reporting;
using TheXDS.Proteus.Resources;
using WpfScreenHelper;

namespace TheXDS.Proteus.Misc
{
    public static class Internal
    {
        private static readonly IEnumerable<IModelSearchFilter> _filters = Objects.FindAllObjects<IModelSearchFilter>().ToList();
        
        public static System.Drawing.Image MakeBarcode(this ModelBase item)
        {
            using var barcode = new BarcodeLib.Barcode();
            return barcode.Encode(BarcodeLib.TYPE.CODE128B, item.StringId);
        }

        public static void ToScreen(this Window window, byte screen)
        {
            var sc = Screen.AllScreens.ToList();
            var s = (screen >= sc.Count ? sc.Last() : sc[screen-1]).Bounds;
            window.Top = s.Top;
            window.Left = s.Left;
            window.Width = s.Width;
            window.Height = s.Height;
            window.WindowState = WindowState.Maximized;
        }

        public static FrameworkElement BuildWarning(string text)
        {
            return new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                MaxWidth = 250,
                Children =
                {
                    new Viewbox
                    {
                        Child = Images.Warning
                    },
                    new TextBlock
                    {
                        Margin = new Thickness(0, 30, 0, 0),
                        Text = text
                    }
                }
            };
        }

        public static IQueryable Query(string query, Type model)
        {
            var s = query.ToLower();
            var f = new List<IFilter>();
            var o = new OrFilter().PushInto(f);

            foreach (var j in _filters)
            {
                if (j.UsableFor(model)) j.AddFilter(f, o, s);
            }

            return QueryBuilder.BuildQuery(model, f);
        }
    }
}
