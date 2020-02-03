/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Windows;
using System.Windows.Controls;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Crud.Mappings.Base;

namespace TheXDS.Proteus.Crud.Mappings
{
    public class TextBoxMapping : ControlBindMapping<TextBox>
    {
        public TextBoxMapping(IPropertyDescription p) : base(p, TextBox.TextProperty)
        {
            if (p is IPropertyTextDescription t)
            {
                if (t.Kind == TextKind.Big)
                {
                    Control.Style = Application.Current.TryFindResource("BigText") as Style;
                    Control.Margin = new Thickness(5);
                }
                if (t.MaxLength > 0) Control.MaxLength = t.MaxLength;
            }
        }
    }
}