using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    /// <summary>
    /// Describe las propiedades Crud para el modelo
    /// <see cref="TablaPrecioItem"/>.
    /// </summary>
    public class TablaPrecioItemDescriptor : CrudDescriptor<TablaPrecioItem>
    {
        /// <summary>
        /// Describe las propiedades Crud para el modelo
        /// <see cref="TablaPrecioItem"/>.
        /// </summary>
        protected override void DescribeModel()
        {
            ObjectProperty(p => p.Item).Selectable();
            this.DescribeValuable();
        }
    }
}