using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.FacturacionUi.ViewModels;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    /// <summary>
    /// Describe las propiedades Crud para el modelo
    /// <see cref="Producto"/>.
    /// </summary>
    public class ProductoDescriptor : CrudDescriptor<Producto, ProductoCrudViewModel>
    {
        /// <summary>
        /// Describe las propiedades Crud para el modelo
        /// <see cref="Producto"/>.
        /// </summary>
        protected override void DescribeModel()
        {
            OnModuleMenu(InteractionType.AdminTool);
            this.DescribeFacturable();
            VmNumericProperty(p => p.PrecioIsv)
                .Range(decimal.Zero, decimal.MaxValue)
                .Important("Precio con ISV");
        }
    }
}