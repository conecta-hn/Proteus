using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    public class SerialBatchDescriptor : BatchDescriptor<SerialBatch>
    {
        protected override void DescribeBatch()
        {
            OnModuleMenu(InteractionType.AdminTool);
            FriendlyName("Bloque de inventario seriado");

            ListProperty(p => p.Serials)
                .Creatable()
                .ShowInDetails()
                .Label("Series");
        }
    }
}