/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Facturacion.Models;

namespace TheXDS.Proteus.Facturacion.Crud
{
    public class TablaPrecioItemDescriptor: CrudDescriptor<TablaPrecioItem>
    {
        protected override void DescribeModel()
        {
            ObjectProperty(p => p.Item).Selectable();
            this.DescribeValuable();
        }
    }
}