using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    /// <summary>
    /// Describe las propiedades Crud para el modelo
    /// <see cref="EstacionBodega"/>.
    /// </summary>
    public class EstacionBodegaCrudDescriptor : CrudDescriptor<EstacionBodega>
    {
        /// <summary>
        /// Describe las propiedades Crud para el modelo
        /// <see cref="EstacionBodega"/>.
        /// </summary>
        protected override void DescribeModel()
        {
            FriendlyName("Enlace Estación/Bodega");

            ObjectProperty(p => p.Estacion).Selectable().Required().Label("Estación").AsListColumn();
            ObjectProperty(p => p.Bodega).Selectable().Required().AsListColumn();
            ShowAllInDetails();
        }
    }

}