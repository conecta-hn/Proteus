using System.Windows.Input;
using TheXDS.MCART.Types.Base;
using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.FacturacionUi.ViewModels
{
    /// <summary>
    /// ViewModel que representa a un ítem dentro de una factura previo a su
    /// creación como una entidad de datos.
    /// </summary>
    public class NewFacturaItemViewModel : NotifyPropertyChanged
    {
        private int _qty;
        private bool _gravar;
        private float _gravado;
        private decimal _descOtor;
        private float _descPercent;

        /// <summary>
        ///     Obtiene o establece el valor Item.
        /// </summary>
        /// <value>El valor de Item.</value>
        public Facturable Item { get; }

        /// <summary>
        ///     Obtiene o establece el valor Qty.
        /// </summary>
        /// <value>El valor de Qty.</value>
        public int Qty
        {
            get => _qty;
            set
            {
                if (Change(ref _qty, value)) Parent.RefreshSubtotals();
            }
        }

        /// <summary>
        ///     Obtiene o establece el valor Gravar.
        /// </summary>
        /// <value>El valor de Gravar.</value>
        public bool Gravar
        {
            get => _gravar;
            set
            {
                if (Change(ref _gravar, value)) Gravado = value ? Item.Isv ?? 0f : 0f;
            }
        }

        /// <summary>
        ///     Obtiene o establece el valor Gravado.
        /// </summary>
        /// <value>El valor de Gravado.</value>
        public float Gravado
        {
            get => _gravado;
            set
            {
                if (Change(ref _gravado, value)) Parent.RefreshSubtotals();
            }
        }

        /// <summary>
        /// Obtiene o establece el valor de descuento otorgado a este ítem.
        /// </summary>
        public decimal DescuentosOtorgados
        {
            get => _descOtor;
            set
            {
                if (Change(ref _descOtor, value))
                {
                    _descPercent = (float)(value * 100 / SubTGrav);
                    Notify(nameof(DescuentosPercent));
                    Parent.RefreshSubtotals();
                }
            }
        }

        /// <summary>
        /// Obtiene o establece el porcentaje de descuento otorgado a este
        /// ítem.
        /// </summary>
        public float DescuentosPercent
        {
            get => _descPercent;
            set
            {
                if (Change(ref _descPercent, value))
                {
                    _descOtor = SubTGrav * ((decimal)value / 100);
                    Notify(nameof(DescuentosOtorgados));
                    Parent.RefreshSubtotals();
                }
            }
        }

        /// <summary>
        /// Obtiene una referencia al objeto padre de esta instancia.
        /// </summary>
        public FacturadorViewModel Parent { get; }

        /// <summary>
        /// Obtiene un valor que indica si es posible establecer un valor de
        /// descuento es esta ítem.
        /// </summary>
        public bool CanDescuento => Parent?.CanDescuento ?? false;

        /// <summary>
        /// Obtiene una referencia estática al precio actual del ítem
        /// facturado.
        /// </summary>
        public decimal Precio => Item.Precio;

        /// <summary>
        /// Calcula el subtotal base de este ítem.
        /// </summary>
        public decimal SubTotal => Item.Precio * Qty;

        /// <summary>
        /// Calcula el monto gravable de este ítem.
        /// </summary>
        public decimal MontoGravado => Gravar ? (SubTotal * (decimal)Gravado / 100m) : 0;

        /// <summary>
        /// Obtiene el subtotal gravado de este ítem.
        /// </summary>
        public decimal SubTGrav => SubTotal + MontoGravado;

        /// <summary>
        /// Calcula el subtotal final de este ítem.
        /// </summary>
        public decimal FinalSubT => SubTGrav - (DescuentosOtorgados + (SubTGrav * ((decimal)DescuentosPercent / 100)));

        /// <summary>
        ///     Obtiene el comando relacionado a la acción RemoveThis.
        /// </summary>
        /// <returns>El comando RemoveThis.</returns>
        public ICommand RemoveThisCommand { get; }

        private void OnRemoveThis()
        {
            Parent.NewItems.Remove(this);
            Parent.RefreshSubtotals();
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="NewFacturaItemViewModel"/>.
        /// </summary>
        /// <param name="parent">Objeto padre de esta instancia.</param>
        /// <param name="item">
        /// Ítem facturable que será representado por esta instancia.
        /// </param>
        public NewFacturaItemViewModel(FacturadorViewModel parent, Facturable item)
        {
            RegisterPropertyChangeBroadcast(nameof(Gravado), nameof(SubTotal), nameof(MontoGravado), nameof(SubTGrav), nameof(FinalSubT));
            RegisterPropertyChangeBroadcast(nameof(Qty), nameof(SubTotal), nameof(MontoGravado), nameof(SubTGrav), nameof(FinalSubT));
            RegisterPropertyChangeBroadcast(nameof(DescuentosOtorgados), nameof(FinalSubT));
            parent.ForwardChange(this);
            Parent = parent;
            Item = item;
            RemoveThisCommand = new SimpleCommand(OnRemoveThis);
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="NewFacturaItemViewModel"/>.
        /// </summary>
        /// <param name="parent">Objeto padre de esta instancia.</param>
        /// <param name="item">Ítem en factura a partir del cual crear esta
        /// instancia.
        /// </param>
        public NewFacturaItemViewModel(FacturadorViewModel parent, ItemFactura item) : this(parent, item.Item)
        {
            Qty = item.Qty;
            Gravar = item.StaticIsv.HasValue;
            Gravado = item.StaticIsv / 100f ?? 0f;
            DescuentosOtorgados = item.StaticDescuento;
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="NewFacturaItemViewModel"/>, estableciendo el ítem de
        /// facturación en la selección actual del ViewModel de facturación.
        /// </summary>
        /// <param name="parent">Objeto padre de esta instancia.</param>
        public NewFacturaItemViewModel(FacturadorViewModel parent) : this(parent, parent.NewItem)
        {
            Qty = parent.NewQty;
            Gravar = parent.NewGravar;
            Gravado = parent.NewGravado;
        }

        /// <summary>
        /// Convierte implícitamente un <see cref="FacturadorViewModel"/> en un
        /// <see cref="NewFacturaItemViewModel"/>.
        /// </summary>
        /// <param name="vm">Objeto a convertir.</param>
        public static implicit operator NewFacturaItemViewModel(FacturadorViewModel vm) => new NewFacturaItemViewModel(vm);

        /// <summary>
        /// Convierte implícitamente un <see cref="NewFacturaItemViewModel"/>
        /// en un <see cref="ItemFactura"/>.
        /// </summary>
        /// <param name="vm">Objeto a convertir.</param>
        public static implicit operator ItemFactura(NewFacturaItemViewModel vm)
        {
            return new ItemFactura
            {
                Item = vm.Item,
                Qty = vm.Qty,
                StaticPrecio = vm.Precio,
                StaticIsv = vm.Gravar ? (float?)vm.Gravado : null
            };
        }

        /// <summary>
        /// Convierte implícitamente un <see cref="NewFacturaItemViewModel"/>
        /// en un <see cref="Facturable"/>.
        /// </summary>
        /// <param name="vm">Objeto a convertir.</param>
        public static implicit operator Facturable(NewFacturaItemViewModel vm) => vm.Item;
    }
}