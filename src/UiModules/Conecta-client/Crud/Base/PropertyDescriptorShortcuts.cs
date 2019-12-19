/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Facturacion.Models.Base;
using System.Linq;
using TheXDS.MCART.Attributes;

namespace TheXDS.Proteus.RrHh.Crud.Base
{
    public static class PropertyDescriptorShortcuts
    {
        [Sugar]
        public static void DescribeFacturable<T>(this CrudDescriptor<T> descriptor) where T : Facturable, new()
        {
            descriptor.CanDelete(p => !p.Instances.Any());
            descriptor.Property(p => p.Name).AsName();
            descriptor.NumericProperty(p => p.Isv)
                .Range(0, 1)
                .Nullable()
                .Label("ISV")
                .ShowInDetails()
                .AsListColumn();

            descriptor.NumericProperty(p => p.Precio)
                .Range(decimal.Zero, decimal.MaxValue)
                .Important("Precio de venta");
        }
    }
}
