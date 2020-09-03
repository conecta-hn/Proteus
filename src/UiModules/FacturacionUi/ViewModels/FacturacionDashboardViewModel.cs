using TheXDS.Proteus.Api;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.ViewModels.Base;

namespace TheXDS.Proteus.FacturacionUi.ViewModels
{
    public class FacturacionDashboardViewModel : ProteusViewModel
    {
        /// <summary>
        ///     Inicializa la clase <see cref="FacturacionDashboardViewModel"/>
        /// </summary>
        public FacturacionDashboardViewModel()
        {
        }

        internal void RefreshDashboard()
        {
            Refresh();
            BusyOp(RefreshVmAsync<FacturacionDashboardViewModel>);
            Proteus.NwClient?.RefreshViewModel<FacturacionDashboardViewModel>();
            Notify(
                nameof(ThisCajaOp),
                "ThisCajaOp.Timestamp",
                "ThisCajaOp.Cajero.UserEntity.Name",
                "ThisCajaOp.OpenBalance",
                "ThisCajaOp.Facturas.Count",
                "ThisCajaOp.TotalFacturas");
        }

        public CajaOp? ThisCajaOp => Proteus.Services is { } ? FacturaService.GetCajaOp : null;
    }
}
