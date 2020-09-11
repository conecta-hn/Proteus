using System.Linq;
using TheXDS.MCART.Attributes;
using TheXDS.MCART.Math;
using TheXDS.MCART.Types.Base;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Crud;
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
            descriptor.Property(p => p.Id).Id("SKU").Tooltip("Código de producto (SKU)");
            descriptor.Property(p => p.Name).AsName();
            descriptor.ObjectProperty(p => p.Category)
                .Selectable().Creatable()
                .Required()
                .Important("Categoría de ítem");

            descriptor.NumericProperty(p => p.Precio)
                .Range(decimal.Zero, decimal.MaxValue)
                .Important("Precio sin ISV");

            descriptor.NumericProperty(p => p.Isv)
                .Range(0f, 100f)
                .Default(15f)
                .Nullable()
                .Label("ISV")
                .ShowInDetails()
                .AsListColumn();

            descriptor.CustomAction("Generar SKU", OnNewSku);

            void OnNewSku(T obj, NotifyPropertyChangeBase vm)
            {
                var tries = 0;
                byte len = 10;
                if (obj.IsNew)
                {
                    do
                    {
                        if (++tries % 10 == 0) len++;
                        obj.Id = GenNewId(len);
                    } while (Proteus.Service<FacturaService>()!.Exists<T>(obj.Id));
                    vm.Notify(nameof(obj.Id));
                }
                else
                {
                    Proteus.MessageTarget?.Stop("No se puede generar un nuevo SKU para un ítem que ya existe.");
                }
            }
        }

        private static string GenNewId(byte len = 10)
        {
            var rnd = new Random();
            var buff = new byte[8];
            rnd.NextBytes(buff);
            var c = BitConverter.ToInt64(buff, 0) & long.MaxValue;
            return c.ToString().PadLeft(len, '0').Substring(0, len);
        }
    }
}