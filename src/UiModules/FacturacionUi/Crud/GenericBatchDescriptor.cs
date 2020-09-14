using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Crud.Base;
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
                .Range(0, int.MaxValue)
                .Label("Cantidad inicial")
                .AsListColumn()
                .Required();

            NumericProperty(p => p.CurrentQty)
                .Range(int.MinValue, 0)
                .Label("Cantidad rebajados")
                .Required();

            CanEdit(p => p.CurrentQty == 0);
        }
    }
}