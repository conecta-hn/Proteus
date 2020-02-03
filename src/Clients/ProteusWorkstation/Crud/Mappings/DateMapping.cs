/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Windows.Controls;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Crud.Mappings.Base;

namespace TheXDS.Proteus.Crud.Mappings
{
    public class DateMapping : ControlBindMapping<DatePicker>
    {
        public DateMapping(IPropertyDescription property) : base(property, DatePicker.SelectedDateProperty)
        {
        }
    }
}