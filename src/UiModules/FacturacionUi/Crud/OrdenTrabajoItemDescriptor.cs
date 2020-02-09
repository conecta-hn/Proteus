using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    /// <summary>
    /// Describe las propiedades Crud para el modelo
    /// <see cref="OrdenTrabajoItem"/>.
    /// </summary>
    public class OrdenTrabajoItemDescriptor : CrudDescriptor<OrdenTrabajoItem>
    {
        /// <summary>
        /// Describe las propiedades Crud para el modelo
        /// <see cref="OrdenTrabajoItem"/>.
        /// </summary>
        protected override void DescribeModel()
        {
            FriendlyName("Ítem de orden de trabajo");
            ObjectProperty(p => p.Item).Selectable().Required().Important();
            NumericProperty(p => p.Qty).Range(1, 1000000).Important("Cantidad");
        }
    }
}