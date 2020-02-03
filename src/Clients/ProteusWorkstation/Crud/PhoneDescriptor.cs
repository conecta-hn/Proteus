/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.Crud
{
    public class PhoneDescriptor : CrudDescriptor<Phone>
    {
        protected override void DescribeModel()
        {
            FriendlyName("Teléfono");
            Property(p => p.PhoneType).Label("Tipo").AsListColumn().ShowInDetails();
            Property(p => p.Number).Important("Número de teléfono").Required();
        }
    }
}