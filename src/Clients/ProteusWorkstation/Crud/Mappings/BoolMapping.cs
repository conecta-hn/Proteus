/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Windows.Controls;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Crud.Mappings.Base;
using System.Windows.Controls.Primitives;

namespace TheXDS.Proteus.Crud.Mappings
{
    public class BoolMapping : ControlBindMapping<CheckBox>
    {
        public BoolMapping(IPropertyDescription p) : base(p, ToggleButton.IsCheckedProperty)
        {
            Control.Content = p.Label;
            if (p.Property.PropertyType.IsClass)
            {
                Control.IsThreeState = true;
            }
        }
    }
}