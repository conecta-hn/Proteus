/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Crud.Mappings.Base;
using TheXDS.Proteus.Models.Base;
using System.Linq;
using System.Windows.Controls;
using TheXDS.MCART.Types.Base;
using System.Windows.Controls.Primitives;
using System.Windows;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Proteus.Crud.Mappings
{
    public class SimpleObjectMapping : ControlBindMapping<ComboBox>, IRefreshable
    {
        private readonly IQueryable<ModelBase> _source;
        public SimpleObjectMapping(IPropertyDescription p) : this(p, Selector.SelectedItemProperty)
        {

        }
        public SimpleObjectMapping(IPropertyDescription p, DependencyProperty dp) : base(p, dp)
        {
            if (p is IDataPropertyDescription i)
            {
                _source = i.Source;
                Control.ItemsSource = i.Source?.ToList() ?? Proteus.Infer(i.PropertyType)?.All(i.PropertyType).ToList();
                Control.DisplayMemberPath = i.DisplayMemberPath;
            }
        }
        public override void ClearControlValue() { }

        public void Refresh()
        {
            Control.ItemsSource = null;
            Control.ItemsSource = _source.ToList();
            if (Control.SelectedItem is ModelBase m && !_source.Contains(m)) ClearControlValue();
        }
    }
}