/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using Xceed.Wpf.Toolkit;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Crud.Mappings.Base;
using System.IO;
using System.Windows;
using static TheXDS.MCART.Types.Extensions.StringExtensions;

namespace TheXDS.Proteus.Crud.Mappings
{
    public class RichTextBoxMapping : PropertyMapping
    {
        private readonly RichTextBox _ctrl;
        public RichTextBoxMapping(IPropertyDescription p) : base (p, new RichTextBox())
        {
            _ctrl = (RichTextBox)Control;
            RichTextBoxFormatBarManager.SetFormatBar(_ctrl, new RichTextBoxFormatBar());
        }

        public override object ControlValue
        {
            get
            {
                using (var ms = new MemoryStream())
                using (var sr = new StreamReader(ms))
                {
                    _ctrl.SelectAll();
                    _ctrl.Selection.Save(ms, DataFormats.Rtf);
                    _ctrl.Selection.Select(_ctrl.Document.ContentEnd, _ctrl.Document.ContentEnd);
                    ms.Position = 0;
                    return sr.ReadToEnd();
                }
            }
            set
            {
                var s = value?.ToString();
                if (s.IsEmpty())
                {
                    ClearControlValue();
                    return;
                }
                _ctrl.SelectAll();
                _ctrl.Selection.Load(s.ToStream(), DataFormats.Rtf);
                _ctrl.Selection.Select(_ctrl.Document.ContentEnd, _ctrl.Document.ContentEnd);
            }
        }

        public override void ClearControlValue()
        {
            _ctrl.Document.Blocks.Clear();
        }
    }
}