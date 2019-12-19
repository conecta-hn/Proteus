/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Facturacion.Models;
using TheXDS.Proteus.Modules;

namespace TheXDS.Proteus.Facturacion.Crud
{
    public class CotizacionDescriptor : CrudDescriptor<Cotizacion>
    {
        protected override void DescribeModel()
        {
            OnModuleMenu(InteractionType.AdminTool);
            FriendlyName("Cotización");

            DateProperty(p => p.Timestamp)
                .WithTime()
                .Default(DateTime.Now)
                .Label("Fecha de creación")
                .AsListColumn()
                .ShowInDetails()
                .ReadOnly();

            DateProperty(p => p.Void)
                .WithTime()
                .Nullable()
                .Default(DateTime.Now)
                .Label("Fecha de vencimiento")
                .AsListColumn()
                .ShowInDetails();

            ObjectProperty(p => p.Cliente)
                .Selectable()
                .Nullable();

            ListProperty(p => p.Items).Creatable();
            Property(p => p.SubTotal).Label("Sub total").ReadOnly();
            Property(p => p.SubTGravable).Label("Sub total gravable").ReadOnly();
            Property(p => p.SubTGravado).Label("Sub total gravado").ReadOnly();
            Property(p => p.SubTFinal).Label("Sub total final").ReadOnly();
            NumericProperty(p => p.Descuentos).Range(0m, decimal.MaxValue);
            NumericProperty(p => p.OtrosCargos).Range(0m, decimal.MaxValue);
            Property(p => p.Total).Label("Total a pagar").ReadOnly();
            TextProperty(p => p.Notas).Big();
            CustomAction("Imprimir", OnPrint);
            CustomAction("Facturar", OnFacturar);
        }

        private void OnFacturar(Cotizacion obj)
        {
            ConectaModule.DefaultFacturaLauncher.Command.Execute((Factura)obj);
        }

        private void OnPrint(Cotizacion obj)
        {
        }
    }
}