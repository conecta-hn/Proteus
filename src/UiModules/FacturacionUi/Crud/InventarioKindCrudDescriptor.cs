using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    /// <summary>
    /// Describe las propiedades Crud para el modelo
    /// <see cref="InventarioKind"/>.
    /// </summary>
    public class InventarioKindCrudDescriptor : CrudDescriptor<InventarioKind>
    {
        /// <summary>
        /// Describe las propiedades Crud para el modelo
        /// <see cref="InventarioKind"/>.
        /// </summary>
        protected override void DescribeModel()
        {
            OnModuleMenu(InteractionType.Catalog);
            FriendlyName("Etiqueta de tipo");
            Property(p => p.Name).AsName().NotEmpty().Required();
        }
    }
}