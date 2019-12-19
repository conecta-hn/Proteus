/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Inventario.Models;

namespace TheXDS.Proteus.Crud
{
    public class CpuInventoryDescriptor : CrudDescriptor<CpuInventory>
    {
        protected override void DescribeModel()
        {
            FriendlyName("Modelos de procesador");
            Property(p => p.Maker).Required().Label("Fabricante").ShowInDetails().AsListColumn();
            Property(p => p.Name).AsName("Modelo").ShowInDetails().AsListColumn();
        }
    }
}