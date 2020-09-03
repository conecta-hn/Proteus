using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.FacturacionUi.ViewModels
{
    /// <summary>
    /// ViewModel que auxilia a representar la información de movimiento de
    /// ítems de inventario desde un Batch.
    /// </summary>
    public class BatchMoveViewModel : ViewModelBase
    {
        private int _Qty;

        /// <summary>
        /// Nombre a mostrar del objeto.
        /// </summary>
        public string DisplayName => Entity.Item.Name;

        /// <summary>
        /// Máximo de elementos que pueden ser movidos del Batch.
        /// </summary>
        public int MaxQty => Entity.Qty;

        /// <summary>
        /// Valor a mostrar para representar el lote relacionado.
        /// </summary>
        public string Lote => Entity.Lote.Id;

        /// <summary>
        /// Batch desde el cual se realizará un movimiento de inventario.
        /// </summary>
        public Batch Entity { get; }

        /// <summary>
        /// Obtiene o establece el valor Qty.
        /// </summary>
        /// <value>El valor de Qty.</value>
        public int Qty
        {
            get => _Qty;
            set => Change(ref _Qty, value);
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="BatchMoveViewModel"/>.
        /// </summary>
        /// <param name="entity">
        /// Batch desde el cual realizar el movimiento de inventario.
        /// </param>
        public BatchMoveViewModel(Batch entity)
        {
            Entity = entity;
        }
    }
}