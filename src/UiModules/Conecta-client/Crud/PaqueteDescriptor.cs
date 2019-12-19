/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Facturacion.Models;
using TheXDS.Proteus.Facturacion.ViewModels;
using TheXDS.Proteus.RrHh.Crud.Base;
using System;

namespace TheXDS.Proteus.Facturacion.Crud
{
    public class PaqueteDescriptor : CrudDescriptor<Paquete, PaqueteCrudViewModel>
    {
        protected override void DescribeModel()
        {
            OnModuleMenu(InteractionType.AdminTool);
            this.DescribeFacturable();
            ListProperty(p => p.Children)
                .Selectable()
                .Label("Ítems contenidos en la promoción");
            DateProperty(p => p.ValidFrom)
                .Required()
                .Label("Válida desde")
                .Default(DateTime.Now);
            DateProperty(p => p.Void)
                .Nullable()
                .Label("Fecha de vencimiento")
                .AsListColumn()
                .ShowInDetails()
                .Default(DateTime.Now.AddMonths(1));

            NumericProperty(p => p.Precio).Hidden();

            VmProperty(p => p.Price)
                .Important("Precio de venta").RadioSelectable();
            VmProperty(p => p.Percent)
                .Important("Porcentaje de descuento").RadioSelectable();

        }
    }
}