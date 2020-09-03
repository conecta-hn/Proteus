using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    public class BatchDescriptor : CrudDescriptor<Batch>
    {
        protected override void DescribeModel()
        {
            OnModuleMenu(InteractionType.AdminTool);
            FriendlyName("Bloque de inventario");

            ObjectProperty(p => p.Bodega)
                .Selectable()
                .Label("Ubicación")
                .Required();

            ObjectProperty(p => p.Item)
                .Selectable()
                .Label("Elemento de inventario")
                .AsListColumn()
                .Required();

            ObjectProperty(p => p.Lote)
                .Creatable()
                .Label("Lote")
                .Required();

            NumericProperty(p => p.Qty)
                .Label("Cantidad")
                .AsListColumn()
                .Required();

            ShowAllInDetails();
            AllListColumn();
        }
    }
}