using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Inventario.Models;

namespace TheXDS.Proteus.Crud
{
    public class GpuInventoryDescriptor : CrudDescriptor<GpuInventory>
    {
        protected override void DescribeModel()
        {
            FriendlyName("Modelos de tarjetas de video");
            Property(p => p.Maker).Required().Label("Fabricante").ShowInDetails().AsListColumn();
            Property(p => p.Name).AsName("Modelo").ShowInDetails().AsListColumn();
        }
    }
}