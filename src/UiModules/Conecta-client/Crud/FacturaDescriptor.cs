/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Facturacion.Models;
using TheXDS.Proteus.Facturacion.ViewModels;
using TheXDS.Proteus.Modules;

namespace TheXDS.Proteus.Facturacion.Crud
{
    public class FacturaDescriptor : CrudDescriptor<Factura, FacturaCrudViewModel>
    {
        protected override void DescribeModel()
        {
            OnModuleMenu(InteractionType.AdminTool);

            VmProperty(p => p.FacturaNumber).OnlyInDetails("Número de factura");
            DateProperty(p => p.Timestamp)
                .WithTime()
                .Default(DateTime.Now)
                .Label("Fecha de creación")
                .AsListColumn()
                .ShowInDetails()
                .ReadOnly();
            ObjectProperty(p => p.Cliente).Selectable().Important().Required();
            ListProperty(p => p.Items).Creatable();
            Property(p => p.SubTotal).Label("Sub total").ReadOnly();
            Property(p => p.SubTGravable).Label("Sub total gravable").ReadOnly();
            Property(p => p.SubTGravado).Label("Sub total gravado").ReadOnly();
            Property(p => p.SubTFinal).Label("Sub total final").ReadOnly();
            NumericProperty(p => p.Descuentos).Range(0m, decimal.MaxValue);
            NumericProperty(p => p.OtrosCargos).Range(0m, decimal.MaxValue);
            Property(p => p.Total).Label("Total a pagar").ReadOnly();
            ListProperty(p => p.Payments).Creatable().Label("Pagos");
            Property(p => p.Paid).ReadOnly();
            TextProperty(p => p.Notas).Big();
            Property(p => p.Impresa).Label("Impresa").ShowInDetails().ReadOnly();
            Property(p => p.Nula).Label("Nula").ShowInDetails().ReadOnly();
            CustomAction("Imprimir factura", OnPrint);
            CustomAction("Anular factura", OnNullify);

            BeforeSave(SetCorrel);
        }

        private void OnPrint(Factura obj)
        {
            if (obj.Impresa)
            {
                ConectaService.PrintFactura(obj, App.Module<ConectaModule>().Interactor);
            }
            else
            {
                ConectaService.AddFactura(obj, true, App.Module<ConectaModule>().Interactor);
            }
            CurrentEditor.SaveCommand.Execute(obj);
        }

        private void OnNullify(Factura obj)
        {
            Proteus.CommonReporter?.UpdateStatus("Anulando factura...");
            obj.Nula = true;
            Proteus.CommonReporter?.Done();
            CurrentEditor.SaveCommand.Execute(obj);
        }

        private void SetCorrel(Factura factura)
        {
            var r = ConectaService.CurrentRango;
            factura.CaiRangoParent = r;
            factura.Correlativo = ConectaService.NextCorrel(r) ?? 0;
        }
    }
}