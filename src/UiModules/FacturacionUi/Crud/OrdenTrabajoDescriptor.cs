using System;
using System.Linq;
using TheXDS.MCART;
using TheXDS.MCART.Types.Base;
using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.FacturacionUi.ViewModels;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    /// <summary>
    /// Describe las propiedades Crud para el modelo
    /// <see cref="OrdenTrabajo"/>.
    /// </summary>
    public class OrdenTrabajoDescriptor : CrudDescriptor<OrdenTrabajo, OrdenTrabajoViewModel>
    {
        /// <summary>
        /// Describe las propiedades Crud para el modelo
        /// <see cref="OrdenTrabajo"/>.
        /// </summary>
        protected override void DescribeModel()
        {
            OnModuleMenu(InteractionType.AdminTool | InteractionType.Essential);
            FriendlyName("Orden de trabajo");
            Property(p => p.Id).ReadOnly();
            DateProperty(p => p.Timestamp)
                .WithTime()
                .Default(DateTime.Now)
                .Label("Fecha de creación")
                .AsListColumn()
                .ShowInDetails()
                .ReadOnly();
            DateProperty(p => p.Entrega)
                .Default(DateTime.Now + TimeSpan.FromDays(2))
                .Label("Fecha de entrega")
                .AsListColumn()
                .ShowInDetails()
                .ReadOnly();
            ObjectProperty(p => p.Cliente)
                .Selectable().Creatable()
                .Important()
                .AsListColumn()
                .ShowInDetails()
                .Required();
            ListProperty(p => p.Items).Creatable().ShowInDetails();
            VmNumericProperty(p => p.DescuentoPercent).Important("Descuentos otorgados");
            NumericProperty(p => p.OtrosCargos).Important("Otros cargos");
            TextProperty(p => p.Notas).Big().ShowInDetails();
            Property(p => p.Facturado).ShowInDetails().AsListColumn().ReadOnly();
            CustomAction("Otorgar descuento de 3ra edad", OnDarDescuento);
            AfterSave(PrintOrden);
        }

        private void OnDarDescuento(OrdenTrabajo ot, NotifyPropertyChangeBase vm)
        {
            if (!Proteus.Service<FacturaService>()!.Elevate(SecurityFlags.Admin))
            {
                Proteus.MessageTarget?.Stop("No tiene permisos para otorgar descuento de tercera edad.");
                return;
            }
            var tot = 0m;
            var exonerar = ot.Cliente?.Exoneraciones.Any(p => DateTime.Today.IsBetween(p.Timestamp, p.Void)) ?? false;
            foreach (var j in ot.Items)
            {
                var precio = j.Item.Precio;
                if (!exonerar)
                    precio += (j.Item.Precio * (decimal)((j.Item.Isv / 100f) ?? 0f));
                tot += precio * j.Qty;
            }
            ot.Descuentos = tot / 1.25m;
            vm.Notify(nameof(ot.Descuentos));
            vm.Notify($"Entity.{nameof(ot.Descuentos)}");            
        }

        private void PrintOrden(OrdenTrabajo arg1, ModelBase arg2)
        {
            FacturaService.PrintOt(arg1);
        }
    }
}