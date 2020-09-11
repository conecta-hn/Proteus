using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    public abstract class BatchDescriptor<T> : CrudDescriptor<T> where T : Batch, new()
    {
        protected override void DescribeModel()
        {
            ObjectProperty(p => p.Bodega)
                .Selectable()
                .Label("Ubicación")
                .Required();

            ObjectProperty(p => p.Item)
                .Selectable().Creatable()
                .Label("Elemento de inventario")
                .AsListColumn()
                .Required();

            ObjectProperty(p => p.Lote)
                .Creatable()
                .Label("Lote")
                .Nullable();

            NumericProperty(p => p.Costo)
                .Range(decimal.Zero,decimal.MaxValue)
                .Label("Costo del ítem");

            DescribeBatch();

            Property(p => p.Qty)
                .ShowInDetails()
                .AsListColumn()
                .Label("Cantidad en existencia")
                .Hidden();

            ShowAllInDetails();
            AllListColumn();
        }

        protected abstract void DescribeBatch();
    }
}