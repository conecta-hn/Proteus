using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.FacturacionUi.ViewModels
{
    public class BatchMoveViewModel : ViewModelBase
    {
        public string DisplayName => Entity.Item.Name;
        public int MaxQty => Entity.Qty;
        public string Lote => Entity.Lote.Id;
        public int Qty { get; set; }
        public Batch Entity { get; }
        public BatchMoveViewModel(Batch entity)
        {
            Entity = entity;
        }
    }
}