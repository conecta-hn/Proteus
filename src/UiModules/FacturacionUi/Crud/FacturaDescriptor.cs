using System;
using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.FacturacionUi.Modules;
using TheXDS.Proteus.FacturacionUi.ViewModels;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    /// <summary>
    /// Describe las propiedades Crud para el modelo
    /// <see cref="Factura"/>.
    /// </summary>
    public class FacturaDescriptor : CrudDescriptor<Factura, FacturaCrudViewModel>
    {
        /// <summary>
        /// Describe las propiedades Crud para el modelo
        /// <see cref="Factura"/>.
        /// </summary>
        protected override void DescribeModel()
        {
            OnModuleMenu(InteractionType.AdminTool);

            VmProperty(p => p.FacturaNumber).AsListColumn().OnlyInDetails("Número de factura");
            DateProperty(p => p.Timestamp)
                .WithTime()
                .Default(DateTime.Now)
                .Label("Fecha de creación")
                .AsListColumn()
                .ShowInDetails()
                .ReadOnly();
            ObjectProperty(p => p.Cliente).Selectable().Important().AsListColumn()
                .ShowInDetails()
                .Required();
            ListProperty(p => p.Items).Creatable().ShowInDetails();
            Property(p => p.SubTotal).AsListColumn()
                .ShowInDetails()
                .Label("Sub total")
                .ReadOnly();
            Property(p => p.SubTGravable).AsListColumn()
                .ShowInDetails()
                .Label("Sub total gravable")
                .ReadOnly();
            Property(p => p.SubTGravado)
                .AsListColumn()
                .ShowInDetails()
                .Label("Sub total gravado")
                .ReadOnly();
            Property(p => p.SubTFinal)
                .AsListColumn()
                .ShowInDetails()
                .Label("Sub total final")
                .ReadOnly();
            NumericProperty(p => p.Descuentos)
                .Range(0m, decimal.MaxValue)
                .AsListColumn()
                .ShowInDetails();
            NumericProperty(p => p.OtrosCargos)
                .Range(0m, decimal.MaxValue)
                .AsListColumn()
                .ShowInDetails();
            Property(p => p.Total).Label("Total a pagar")
                .AsListColumn()
                .ShowInDetails()
                .ReadOnly();
            ListProperty(p => p.Payments).Creatable().Label("Pagos");
            Property(p => p.Paid)
                .AsListColumn()
                .ShowInDetails()
                .ReadOnly();
            TextProperty(p => p.Notas).Big().ShowInDetails();
            Property(p => p.Impresa).Label("Impresa").AsListColumn().ShowInDetails().ReadOnly();
            Property(p => p.Nula).Label("Nula").AsListColumn().ShowInDetails().ReadOnly();
            CustomAction("Imprimir factura", OnPrint);
            CustomAction("Anular factura", OnNullify);

            BeforeSave(SetCorrel).Then(SetCajaOp);
        }

        private void OnPrint(Factura obj)
        {
            if (obj.Impresa)
            {
                FacturaService.PrintFactura(obj, App.Module<FacturacionModule>().Interactor);
            }
            else
            {
                FacturaService.AddFactura(obj, true, App.Module<FacturacionModule>().Interactor);
            }
            CurrentEditor?.SaveCommand.Execute(obj);
        }

        private async void OnNullify(Factura obj)
        {
            obj.Nula = true;
            CurrentEditor?.SaveCommand.Execute(obj);
        }

        private void SetCajaOp(Factura obj)
        {
            obj.Parent = FacturaService.GetCajaOp;
        }

        private void SetCorrel(Factura factura)
        {
            var r = FacturaService.CurrentRango;
            factura.CaiRangoParent = r;
            factura.Correlativo = FacturaService.NextCorrel(r) ?? 0;
        }
    }
}