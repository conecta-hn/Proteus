using System;
using System.Linq;
using TheXDS.MCART.Types.Base;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Dialogs;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.ViewModels.Base;

namespace TheXDS.Proteus.FacturacionUi.Crud
{
    /// <summary>
    /// Describe las propiedades Crud para el modelo
    /// <see cref="CajaOp"/>.
    /// </summary>
    public class CajaOpDescriptor : CrudDescriptor<CajaOp>
    {
        /// <summary>
        /// Describe las propiedades Crud para el modelo
        /// <see cref="CajaOp"/>.
        /// </summary>
        protected override void DescribeModel()
        {
            OnModuleMenu(Annotations.InteractionType.AdminTool);
            FriendlyName("Sesión de caja");

            ObjectProperty(p => p.Estacion).Selectable().AsListColumn().ShowInDetails().Label("Estación");
            ObjectProperty(p => p.Cajero).Selectable().AsListColumn().ShowInDetails();
            NumericProperty(p => p.OpenBalance).Positive().Label("Balance de apertura").AsListColumn().ShowInDetails();

            ListProperty(p => p.Facturas).Creatable();

            Property(p => p.TotalFacturas).Label("Total facturado").AsListColumn().ShowInDetails().ReadOnly();
            Property(p => p.TotalEfectivo).Label("Total facturado (sólo efectivo)").AsListColumn().ShowInDetails().ReadOnly();

            Property(p => p.CloseTimestamp).Label("Fecha/hora de cierre").AsListColumn().ShowInDetails().ReadOnly();
            Property(p => p.CloseBalance).Label("Balance de cierre").AsListColumn().ShowInDetails().ReadOnly();
            CustomAction("Cerrar sesión de caja", OnCloseSession);
        }

        private void OnCloseSession(CajaOp cajaOp, NotifyPropertyChangeBase vm)
        {
            if (cajaOp.CloseTimestamp.HasValue)
            {
                Proteus.MessageTarget?.Stop("La caja ya está cerrada.");
                return;
            }
            if (!InputSplash.GetNew<decimal>("Cuente el dinero de la caja, e introduzca el total en efectivo.", out var cierre)) return;
            var totalEfectivo = cajaOp.Facturas.Sum(p => p.TotalPagadoEfectivo);
            var cuadre = cajaOp.OpenBalance + totalEfectivo - cierre;
            if (cuadre != 0)
            {
                Proteus.MessageTarget?.Warning($"El cierre de caja no cuadra por {cuadre:C}.");
                return;
            }
            cajaOp.CloseBalance = cierre;
            cajaOp.CloseTimestamp = DateTime.Now;
            (vm as ICrudViewModel)?.SaveCommand.Execute(cajaOp);
            Proteus.MessageTarget?.Info($"Caja cerrada correctamente. Debe depositar {cierre - cajaOp.Cajero.OptimBalance:C} para mantener su fondo de caja de {cajaOp.Cajero.OptimBalance}");
        }
    }
}