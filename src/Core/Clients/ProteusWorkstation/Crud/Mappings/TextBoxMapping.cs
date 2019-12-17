/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Windows;
using System.Windows.Controls;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Crud.Mappings.Base;
using Xceed.Wpf.Toolkit;
using static TheXDS.MCART.Types.Extensions.StringExtensions;

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

    public class MaskedTextBoxMapping : PropertyMapping
    {
        protected new MaskedTextBox Control => base.Control as MaskedTextBox;

        public MaskedTextBoxMapping(IPropertyDescription p) : base(p, new MaskedTextBox())
        {
            if (p is IPropertyTextDescription t)
            {
                if (t.Kind == TextKind.Big)
                {
                    Control.Style = Application.Current.TryFindResource("BigText") as Style;
                    Control.Margin = new Thickness(5);
                }
                if (t.MaxLength > 0) Control.MaxLength = t.MaxLength;
                if (!t.Mask.IsEmpty()) Control.Mask = t.Mask;
            }
        }

        public override object ControlValue
        {
            get => Control.IsMaskCompleted ? Control.Text : null;
            set => Control.Text = value as string;
        }

        public override void ClearControlValue()
        {
            Control.Text = null;
        }
    }
}