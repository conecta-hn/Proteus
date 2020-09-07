using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Models;
using TheXDS.MCART.Types;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.ViewModels.Base;
using System.Linq;
using TheXDS.Proteus.Component;
using System.Threading.Tasks;

namespace TheXDS.Proteus.FacturacionUi.ViewModels
{
    public class InventarioMoveViewModel : ServicingPageViewModel<FacturaService>
    {
        private Bodega? _origen;
        private Bodega? _destino;

        public ObservableListWrap<Bodega> Bodegas { get; private set; } = null!;

        public Bodega? Origen
        {
            get => _origen;
            set
            {
                if (Change(ref _origen, value))
                {
                    OrigenItems.Substitute(Origen?.Batches.Select(p => new BatchMoveViewModel(p)).ToList()!);
                }
            }
        }

        public Bodega? Destino
        {
            get => _destino;
            set => Change(ref _destino, value);
        }

        public ObservableListWrap<BatchMoveViewModel> OrigenItems { get; } = new ObservableListWrap<BatchMoveViewModel>();

        /// <summary>
        /// Obtiene el comando relacionado a la acción Switch.
        /// </summary>
        /// <returns>El comando Switch.</returns>
        public SimpleCommand SwitchCommand { get; }

        /// <summary>
        /// Obtiene el comando relacionado a la acción MoveInventario.
        /// </summary>
        /// <returns>El comando MoveInventario.</returns>
        public SimpleCommand MoveInventarioCommand { get; }

        public InventarioMoveViewModel(ICloseable host) : base(host)
        {
            Title = "Movimiento de inventario";
            SwitchCommand = new SimpleCommand(OnSwitch);
            MoveInventarioCommand = new SimpleCommand(() => BusyOp(OnMoveInventario));
        }

        protected override async Task OnStartup()
        {
            Bodegas = await GetObservableAsync<Bodega>();
        }

        private void OnSwitch()
        {
            var tmp = Origen;
            Origen = Destino;
            Destino = tmp;
        }
        private async void OnMoveInventario()
        {
            if (!OrigenItems?.Any() ?? true)
            {
                Proteus.MessageTarget?.Stop("Debe seleccionar una bodega de origen que contenga items.");
                return;
            }
            if (Destino is null)
            {
                Proteus.MessageTarget?.Stop("Debe seleccionar una bodega de destino.");
                return;
            }
            if (Origen == Destino)
            {
                Proteus.MessageTarget?.Stop("Esta operación no es válida.");
                return;
            }

            VirtualMove();
            await Service.SaveAsync();
            Proteus.MessageTarget?.Info("Movimiento de inventario realizado correctamente.");
        }

        private void VirtualMove()
        {
            var rem = new ListEx<BatchMoveViewModel>();

            foreach (var j in OrigenItems)
            {
                if (j.Qty == 0) continue;
                if (j.Qty == j.MaxQty)
                {
                    j.Entity.Bodega = Destino!;
                    rem.Add(j);
                }
                else
                {
                    var newBatch = j.Entity.Split(j.Qty);
                    Destino!.Batches.Add(newBatch);
                    j.Notify(nameof(j.MaxQty));
                    j.Qty = 0;
                }
            }
            foreach(var j in rem)
            {
                OrigenItems.Remove(j);
            }
        }
    }
}