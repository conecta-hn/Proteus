/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Crud.Mappings.Base;
using TheXDS.Proteus.Widgets;

namespace TheXDS.Proteus.Crud.Mappings
{
    public class PicturePickerMapping : ControlBindMapping<PicturePicker>
    {
        public PicturePickerMapping(IPropertyDescription p) : base(p, PathPickerBase.PathProperty)
        {
        }
    }
}