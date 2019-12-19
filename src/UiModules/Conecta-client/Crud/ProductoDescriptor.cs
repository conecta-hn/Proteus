/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Facturacion.Models;
using TheXDS.Proteus.RrHh.Crud.Base;

namespace TheXDS.Proteus.Facturacion.Crud
{
    public class ProductoDescriptor : CrudDescriptor<Producto>
    {
        protected override void DescribeModel()
        {
            OnModuleMenu(InteractionType.AdminTool);
            this.DescribeFacturable();
            ListProperty(p => p.Consumos).Creatable().Label("Consumos de inventario");
        }
    }
}