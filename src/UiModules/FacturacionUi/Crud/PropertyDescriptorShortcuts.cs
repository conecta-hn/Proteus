using System;
using TheXDS.MCART.Attributes;
using TheXDS.MCART.Types.Base;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Api;
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

            descriptor.BeforeSave(SetNewSku);
        }

        private static void SetNewSku(Facturable obj)
        {
            var tries = 0;
            byte len = 11;
            if (obj.IsNew && obj.Id.IsEmpty())
            {
                do
                {
                    if (++tries % 10 == 0) len++;
                    obj.Id = GenNewId(len);
                } while (Proteus.Service<FacturaService>()!.Exists(typeof(Facturable), obj.Id));
                Proteus.MessageTarget?.Info($"SKU generado automáticamente: {obj.Id}");
            }
        }
        private static string GenNewId(byte len = 11)
        {
            var rnd = new Random();
            var buff = new byte[8];
            rnd.NextBytes(buff);
            var c = BitConverter.ToInt64(buff, 0) & long.MaxValue;
            return c.ToString().PadLeft(len, '0').Substring(0, len);
        }
    }
}