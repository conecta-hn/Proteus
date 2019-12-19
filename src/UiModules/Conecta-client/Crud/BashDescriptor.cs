using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Inventario.Models;

namespace TheXDS.Proteus.Crud
{
    public class BatchDescriptor : CrudDescriptor<Batch>
    {
        protected override void DescribeModel()
        {
            FriendlyName("Bloque de inventario");

            ObjectProperty(p => p.Bodega)
                .Selectable()
                .Label("Ubicación")
                .Required();

            ObjectProperty(p => p.Proveedor)
                .Selectable()
                .Required();

            ObjectProperty(p => p.Definition)
                .Selectable()
                .Label("Modelo de equipo")
                .Required();

            ListProperty(p => p.Items)
                .Creatable()
                .Label("Ítems")
                .NotEmpty()
                .ShowInDetails();
        }
    }
}
