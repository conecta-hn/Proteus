/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Facturacion.Models;

namespace TheXDS.Proteus.Facturacion.Crud
{
    public class TablaPrecioDescriptor : CrudDescriptor<TablaPrecio>
    {
        protected override void DescribeModel()
        {
            OnModuleMenu(InteractionType.AdminTool);
            Property(p => p.Name).AsName();
            NumericProperty(p => p.GlobalDescuento)
                .Range(0f, 100f)
                .Nullable()
                .Label("Descuento general");
            ListProperty(p => p.Items).Creatable();
            ListProperty(p => p.AppliedTo).Selectable().Label("Aplicado a las categorías");
        }
    }
}