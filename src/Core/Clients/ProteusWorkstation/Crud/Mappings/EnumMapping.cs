/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Crud.Mappings.Base;
using System.Windows.Controls;
using TheXDS.MCART.Types.Extensions;
using System.Windows.Controls.Primitives;
using static TheXDS.MCART.Types.Extensions.NamedObjectExtensions;

namespace TheXDS.Proteus.Crud.Mappings
{

    public class EnumMapping : PropertyMapping
    {
        private readonly ComboBox _cmb;
        private static ComboBox MkComboBox(IPropertyDescription property)
        {
            var c = new ComboBox
            {
                ItemsSource = property.Property.PropertyType.AsNamedEnum(),
                SelectedValuePath = "Value",
                DisplayMemberPath = "Name"
            };
            c.SetBinding(Selector.SelectedValueProperty, property.Property.Name);
            return c;
        }
        public EnumMapping(IPropertyDescription property) : base(property, MkComboBox(property))
        {
            _cmb = (ComboBox)Control;
        }

        public override object ControlValue
        {
            get => _cmb.SelectedValue;
            set => _cmb.SelectedValue = value;
        }

        public override void ClearControlValue() => _cmb.SelectedValue = null;
    }
}