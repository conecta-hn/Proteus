using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    public class GenericBatchDescriptor : BatchDescriptor<GenericBatch>
    {
        protected override void DescribeBatch()
        {
            OnModuleMenu(InteractionType.AdminTool);
            FriendlyName("Bloque de inventario generico");

            NumericProperty(p => p.InitialQty)
                .Label("Cantidad")
                .AsListColumn()
                .Required();

            CanEdit(p => p.CurrentQty == 0);
        }
    }
}