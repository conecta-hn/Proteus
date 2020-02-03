using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    /// <summary>
    /// Describe las propiedades Crud para el modelo
    /// <see cref="TablaPrecio"/>.
    /// </summary>
    public class TablaPrecioDescriptor : CrudDescriptor<TablaPrecio>
    {
        /// <summary>
        /// Describe las propiedades Crud para el modelo
        /// <see cref="TablaPrecio"/>.
        /// </summary>
        protected override void DescribeModel()
        {
            OnModuleMenu(InteractionType.AdminTool);
            Property(p => p.Name).AsName();
            ListProperty(p => p.Items).Creatable();
            ListProperty(p => p.AppliedTo).Selectable().Label("Aplicado a las categorías");
        }
    }
}