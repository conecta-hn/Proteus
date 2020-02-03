using System.Linq;
using TheXDS.MCART.Attributes;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    /// <summary>
    /// Define accesos directos de descripción rápida para generar ventanas de
    /// Crud.
    /// </summary>
    public static class PropertyDescriptorShortcuts
    {
        /// <summary>
        /// Describe rápidamente las propiedades Crud para los modelos que
        /// hereden de la clase <see cref="Facturable"/>.
        /// </summary>
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