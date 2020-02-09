using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    /// <summary>
    /// Describe las propiedades Crud para el modelo
    /// <see cref="FacturableCategory"/>.
    /// </summary>
    public class FacturableCategoryDescriptor : CrudDescriptor<FacturableCategory>
    {
        /// <summary>
        /// Describe las propiedades Crud para el modelo
        /// <see cref="FacturableCategory"/>.
        /// </summary>
        protected override void DescribeModel()
        {
            OnModuleMenu(InteractionType.Settings);
            FriendlyName("Categoría de producto");
            Property(p => p.Name).AsName();
        }
    }
}