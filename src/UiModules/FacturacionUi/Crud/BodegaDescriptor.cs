using System.Linq;
using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    public class BodegaDescriptor : CrudDescriptor<Bodega>
    {
        protected override void DescribeModel()
        {
            OnModuleMenu(InteractionType.Settings);
            Property(p => p.Name).AsName("Nombre de bodega");

            ListProperty(p => p.Batches)
                .Editable()
                .Creatable()
                .Label("Bloques de inventario")
                .ShowInDetails();

            CanDelete(b => !b?.Batches?.Any() ?? false);
        }
    }
}