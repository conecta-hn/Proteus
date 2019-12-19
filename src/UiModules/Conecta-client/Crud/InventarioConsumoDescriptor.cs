/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Facturacion.Models;
using TheXDS.Proteus.Inventario.Models;

namespace TheXDS.Proteus.Facturacion.Crud
{
    public class InventarioConsumoDescriptor : CrudDescriptor<InventarioConsumo>
    {
        protected override void DescribeModel()
        {
            FriendlyName("Consumo de inventario");

            LinkProperty<EquipoInstance>(p => p.InventarioItemId)
                .Important("Ítem de inventario");

            NumericProperty(p => p.Qty)
                .Range(1, 1000000)
                .Default(1)
                .Important("Cantidad");
        }
    }
}