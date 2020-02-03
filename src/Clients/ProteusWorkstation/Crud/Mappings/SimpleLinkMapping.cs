/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Base;
using System.Windows.Controls.Primitives;

namespace TheXDS.Proteus.Crud.Mappings
{

    public class SimpleLinkMapping : SimpleObjectMapping
    {
        public SimpleLinkMapping(IPropertyDescription p) : base(p, Selector.SelectedValueProperty)
        {
            Control.SelectedValuePath = "Id";
        }
    }
}