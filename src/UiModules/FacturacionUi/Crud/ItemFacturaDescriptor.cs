using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.FacturacionUi.ViewModels;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    /// <summary>
    /// Describe las propiedades Crud para el modelo
    /// <see cref="ItemFactura"/>.
    /// </summary>
    public class ItemFacturaDescriptor : CrudDescriptor<ItemFactura, ItemFacturaCrudViewModel>
    {
        /// <summary>
        /// Describe las propiedades Crud para el modelo
        /// <see cref="ItemFactura"/>.
        /// </summary>
        protected override void DescribeModel()
        {
            FriendlyName("Ítem de factura");
            VmObjectProperty(p => p.ActualItem).Selectable().Label("Ítem");
            ObjectProperty(p => p.Item).AsListColumn().OnlyInDetails();
            NumericProperty(p => p.Qty).Range(1, 1000000).Important("Cantidad");
            VmNumericProperty(p => p.Precio).Range(decimal.Zero, decimal.MaxValue).Important("Precio de venta");
            VmNumericProperty(p => p.Descuento).Range(decimal.Zero, decimal.MaxValue).Important("Descuento individual otorgado");
            VmNumericProperty(p => p.Isv).Range(0, 100).Nullable().Label("ISV");
        }
    }
}