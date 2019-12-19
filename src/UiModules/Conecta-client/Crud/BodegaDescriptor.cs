/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Inventario.Models;

namespace TheXDS.Proteus.Crud
{
    public class BodegaDescriptor : CrudDescriptor<Bodega>
    {
        protected override void DescribeModel()
        {
            Property(p => p.Name).AsName("Nombre de bodega");

            ListProperty(p => p.Batches)
                .Editable()
                .Creatable()
                .Label("Bloques de inventario")
                .ShowInDetails();
        }
    }
}