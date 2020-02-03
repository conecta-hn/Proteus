using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    /// <summary>
    /// Describe las propiedades Crud para el modelo
    /// <see cref="Producto"/>.
    /// </summary>
    public class ProductoDescriptor : CrudDescriptor<Producto>
    {
        /// <summary>
        /// Describe las propiedades Crud para el modelo
        /// <see cref="Producto"/>.
        /// </summary>
        protected override void DescribeModel()
        {
            OnModuleMenu(InteractionType.AdminTool);
            this.DescribeFacturable();
            //ListProperty(p => p.Consumos).Creatable().Label("Consumos de inventario");
        }
    }
}