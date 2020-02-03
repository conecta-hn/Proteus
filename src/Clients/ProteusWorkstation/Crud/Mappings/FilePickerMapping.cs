/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Crud.Mappings.Base;
using TheXDS.Proteus.Widgets;

namespace TheXDS.Proteus.Crud.Mappings
{
    public class FilePickerMapping : ControlBindMapping<FilePicker>
    {
        public FilePickerMapping(IPropertyDescription p) : base(p, PathPickerBase.PathProperty)
        {
            if (p is IPropertyTextDescription)
            {
                Control.Filters.Add(new FileFilter("Todos los archivos", "*"));
            }
        }
    }
}