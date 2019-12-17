using System.Collections;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Proteus.Widgets
{
    public class ResourcesTemplateSelector : DataTemplateSelector
    {
        public static ResourcesTemplateSelector Create => new ResourcesTemplateSelector();
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var n = item?.GetType().ResolveToDefinedType().NameOf();

            if (n.IsEmpty()) return null;
            Debug.Assert(n != null, nameof(n) + " != null");


            if (container is TreeViewItem t)
            {
                var trv = (DependencyObject)t;
                TreeView tree = null;
                while (tree is null)
                {
                    trv = VisualTreeHelper.GetParent(trv);
                    if (trv is null) break;
                    tree = trv as TreeView;
                }
                container = tree;
            }

            return
                (container as FrameworkElement)?.TryFindResource(n) as DataTemplate ??
                FindByType(container as FrameworkElement, n) ??
                Application.Current?.TryFindResource(n) as DataTemplate;
        }

        private DataTemplate FindByType(FrameworkElement f, string key)
        {
            foreach (DictionaryEntry j in f.Resources)
            {
                switch (j.Key)
                {
                    case string s:
                        if (s.Contains(key)) return j.Value as DataTemplate;
                        break;
                    case DataTemplateKey dtk:
                        if (dtk.DataType.ToString().Contains(key)) return j.Value as DataTemplate;
                        break;
                }
            }

            return null;
        }
    }
}
