/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using TheXDS.MCART.Controls;
using TheXDS.MCART.Types.Base;
using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Crud.Mappings;
using TheXDS.Proteus.Models.Base;
using static TheXDS.MCART.Types.Extensions.CollectionExtensions;
using static TheXDS.MCART.Types.Extensions.TypeExtensions;

namespace TheXDS.Proteus.Crud
{
    internal static class CrudBuilder
    {
        internal static FrameworkElement BuildEditor(IEntityViewModel parentVm, ICrudDescription d, out ICollection<IPropertyMapping> boxes, Type? parentEntityType = null)
        {
            boxes = new HashSet<IPropertyMapping>();
            var stckpnl = App.UiInvoke(() => new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center,
                Style = Application.Current.TryFindResource("BotoneraBase") as Style,
                Language = System.Windows.Markup.XmlLanguage.GetLanguage(System.Globalization.CultureInfo.CurrentCulture.Name)
            });
            foreach (var j in d.Descriptions)
            {
                if (j.Hidden) continue;
                if (j.PropertyType.ResolveCollectionType() == parentEntityType) continue;

                stckpnl.Children.Add(boxes.Push(PropertyMapper.GetMapping(parentVm, j)).ContainingControl);
            }
            var wp = new StretchyWrapPanel { HorizontalAlignment = HorizontalAlignment.Center };
            var addwp = false;
            foreach (var j in d.CustomActions)
            {
                addwp = true;
                var b = new Button { Content = j.Key };
                b.Click += (sender, e) => j.Value((stckpnl.DataContext as IEntityViewModel)?.Entity as ModelBase, stckpnl.DataContext as NotifyPropertyChangeBase);
                wp.Children.Add(b);
            }
            if (addwp) stckpnl.Children.Add(wp);
            return stckpnl;
        }

        internal static FrameworkElement BuildDetails(Type model, ICrudDescription d)
        {
            var tb = new TextBlock()
            {
                Margin = new Thickness(0, 30, 0, 0),
                Style = Application.Current.TryFindResource("Title") as Style
            };

            if (model.Implements<INameable>())
            {
                tb.SetBinding(TextBlock.TextProperty, new Binding($"Entity.{nameof(INameable.Name)}"));
            }
            else
            {
                var r = new Run();
                r.SetBinding(Run.TextProperty, new Binding("Entity.Id"));
                tb.Inlines.Add($"{d.FriendlyName} #");
                tb.Inlines.Add(r);
            }

            var stckPnl = new StackPanel
            {
                Language = System.Windows.Markup.XmlLanguage.GetLanguage(System.Globalization.CultureInfo.CurrentCulture.Name),
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Style = Application.Current.TryFindResource("BotoneraBase") as Style,
                Children =
                {
                    tb,
                    new Separator()
                }
            };

            foreach (var j in d.Descriptions)
            {
                if (!j.ShowInDetails) continue;
                stckPnl.Children.Add(new ReadOnlyMapping(j).Control);
            }

            return stckPnl;
        }

        internal static FrameworkElement BuildWarning(Type model)
        {
            return Misc.AppInternal.BuildWarning($"No se encontró ningún descriptor de CRUD para el modelo '{model.Name}'.");
        }

        internal static bool DescribesModel(Type t, Type model)
        {
            try { return t.BaseType.GenericTypeArguments.First() == model; }
            catch { return false; }
        }
    }
}