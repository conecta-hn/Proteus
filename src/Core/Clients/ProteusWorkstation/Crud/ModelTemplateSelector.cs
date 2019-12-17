/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Base;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using static TheXDS.MCART.Types.Extensions.TypeExtensions;

namespace TheXDS.Proteus.Crud
{
    internal class ModelTemplateSelector : DataTemplateSelector
    {
        private readonly IList<ICrudDescription> _descriptions;
        public ModelTemplateSelector(IEnumerable<ICrudDescription> descriptions)
        {
            _descriptions = descriptions.ToList();
        }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return _descriptions.FirstOrDefault(p => p.DescribedModel == item.GetType().ResolveToDefinedType())?.TreeViewTemplate;
        }
    }
}