using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    /// <summary>
    /// Describe las propiedades Crud para el modelo
    /// <see cref="InventarioKindItem"/>.
    /// </summary>
    public class InventarioKindItemCrudDescriptor : CrudDescriptor<InventarioKindItem>
    {
        /// <summary>
        /// Describe las propiedades Crud para el modelo
        /// <see cref="InventarioKindItem"/>.
        /// </summary>
        protected override void DescribeModel()
        {
            FriendlyName("Ítem de etiqueta de inventario");

            ObjectProperty(p => p.Kind)
                .Selectable()
                .Required()
                .Label("Etiqueta")
                .ShowInDetails();

            ObjectProperty(p => p.Producto)
                .Selectable()
                .Required()
                .ShowInDetails();

        }
    }
}