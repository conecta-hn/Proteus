/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Windows.Controls;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Crud.Mappings.Base;
using System.Windows.Controls.Primitives;
using System.Linq;
using System.Windows.Data;

namespace TheXDS.Proteus.Crud.Mappings
{
    public class SimpleObjectSelectorMapping : PropertyMapping
    {
        private readonly ComboBox _cmb;

        public SimpleObjectSelectorMapping(IPropertyDescription property) : base(property, new ComboBox())
        {
            _cmb = (ComboBox)Control;

            if (property is IObjectPropertyDescription p)
            {
                _cmb.ItemsSource = p.Source.ToList();                
            }
            _cmb.SetBinding(Selector.SelectedItemProperty, new Binding(property.Property.Name));
        }

        public override object ControlValue
        {
            get => _cmb.SelectedItem;
            set { }
        }

        public override void ClearControlValue()
        {
        }
    }
}