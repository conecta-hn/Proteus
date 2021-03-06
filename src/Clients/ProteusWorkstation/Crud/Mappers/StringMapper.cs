﻿/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Mappers.Base;
using TheXDS.Proteus.Crud.Mappings;
using TheXDS.Proteus.Crud.Base;
using TheXDS.MCART.ViewModel;

namespace TheXDS.Proteus.Crud.Mappers
{
    public sealed class StringMapper : SimpleMapper<string>
    {
        public override IPropertyMapping Map(IEntityViewModel parentVm, IPropertyDescription p)
        {

            switch ((p as IPropertyTextDescription)?.Kind)
            {
                case TextKind.PicturePath:
                    return new PicturePickerMapping(p);
                case TextKind.FilePath:
                    return new FilePickerMapping(p);
                case TextKind.Maskable:
                    return new MaskedTextBoxMapping(p);
                case TextKind.Rich:
                    return new RichTextBoxMapping(p);
                default:
                    return new TextBoxMapping(p);
            }
        }
    }
}