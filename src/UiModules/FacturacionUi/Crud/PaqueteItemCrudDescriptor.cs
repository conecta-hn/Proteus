using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    /// <summary>
    /// Describe las propiedades Crud para el modelo
    /// <see cref="PaqueteItem"/>.
    /// </summary>
    public class PaqueteItemCrudDescriptor : CrudDescriptor<PaqueteItem>
    {
        /// <summary>
        /// Describe las propiedades Crud para el modelo
        /// <see cref="PaqueteItem"/>.
        /// </summary>
        protected override void DescribeModel()
        {
            FriendlyName("Ítem del paquete");

            ObjectProperty(p => p.Item)
                .Selectable()
                .Required()
                .ShowInDetails()
                .AsListColumn()
                .Label("Ítem");

            ObjectProperty(p => p.Parent)
                .Selectable()
                .Required()
                .ShowInDetails()
                .AsListColumn()
                .Label("Paquete");
        }
    }

}