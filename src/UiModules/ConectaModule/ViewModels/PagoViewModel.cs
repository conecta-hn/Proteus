using System;
using System.Linq;
using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Conecta.Models;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Conecta.ViewModels
{
    public abstract class PagoViewModel<T> : DynamicViewModel<T> where T: ModelBase, IPagable, new()
    {
        public decimal Pendiente => (Entity?.Total ?? 0m) - Abonado;
        
        public decimal Abonado => (Entity?.Pagos.Any() ?? false) ? Entity.Pagos.Sum(p => p.Abono) : 0m;
        
        public Pago? LastPago
        {
            get
            {
                if (Pendiente == 0m || !Entity!.Pagos.Any()) return null;
                return Entity!.Pagos.OrderBy(p => p.Timestamp).Last();
            }
        }

        public string LastPagoWhen => (Entity?.IsNew ?? true) ? "sin datos." : $"Hace {(int)(DateTime.Now - (LastPago?.Timestamp ?? Entity.Timestamp)).TotalDays} días";
        
        public decimal? LastPagoHowMuch => LastPago?.Abono;

        public PagoViewModel()
        {
            RegisterPropertyChangeBroadcast(
                nameof(Entity.Total),
                nameof(Pendiente));
            RegisterPropertyChangeBroadcast(
                nameof(Entity.Pagos),
                nameof(Abonado),
                nameof(LastPago),
                nameof(LastPagoWhen),
                nameof(LastPagoHowMuch));
        }
    }
}
