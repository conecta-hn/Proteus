using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    /// <summary>
    /// Describe las propiedades Crud para el modelo
    /// <see cref="ProductoProveedor"/>.
    /// </summary>
    public class ProductoProveedorCrudDescriptor : CrudDescriptor<ProductoProveedor>
    {
        /// <summary>
        /// Describe las propiedades Crud para el modelo
        /// <see cref="ProductoProveedor"/>.
        /// </summary>
        protected override void DescribeModel()
        {
            FriendlyName("Proveedor de producto");
            ObjectProperty(p => p.Producto)
                .Selectable()
                .Required()
                .ShowInDetails();

            ObjectProperty(p => p.Producto)
                .Selectable()
                .Required()
                .ShowInDetails();
        }
    }

}