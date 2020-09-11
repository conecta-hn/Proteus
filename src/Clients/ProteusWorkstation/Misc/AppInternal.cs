/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.Resources;

namespace TheXDS.Proteus.Misc
{
    public static class AppInternal
    {
        public static Type[] GetModels(Type baseModel)
        {
            return baseModel.Derivates()
                .Select(p => p.ResolveToDefinedType()!)
                .Where(p => p.IsInstantiable())
                .Distinct()
                .ToArray();
        }

        public static ICollection<ModelBase>? GetSource(IEnumerable<ModelBase>? source)
        {
            return source switch
            {
                ObservableListWrap<ModelBase> o => o,
                ObservableCollectionWrap<ModelBase> o => o,
                null => null,
                _ => source.ToList()
            };
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

        public static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
