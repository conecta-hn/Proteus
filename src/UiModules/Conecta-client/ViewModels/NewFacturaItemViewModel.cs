/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Facturacion.Models;
using TheXDS.Proteus.Facturacion.Models.Base;
using System.Windows.Input;
using TheXDS.MCART.Types.Base;
using TheXDS.MCART.ViewModel;

namespace TheXDS.Proteus.Facturacion.ViewModels
{
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
                if (Change(ref _gravar, value) && !value) Gravado = 0f;
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

        public FacturadorViewModel Parent { get; }

        public bool CanDescuento => Parent?.CanDescuento ?? false;

        public decimal Precio => Item.Precio;
        public decimal SubTotal => Item.Precio * Qty;
        public decimal MontoGravado => Gravar ? (SubTotal * (decimal)Gravado) : 0;
        public decimal SubTGrav => SubTotal + MontoGravado;        
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

        public NewFacturaItemViewModel(FacturadorViewModel parent, ItemFactura item): this(parent,item.Item)
        {
            Qty = item.Qty;
            Gravar = item.StaticIsv.HasValue;
            Gravado = item.StaticIsv ?? 0f;
            DescuentosOtorgados = item.StaticDescuento;
        }

        public NewFacturaItemViewModel(FacturadorViewModel parent) : this(parent, parent.NewItem)
        {
            Qty = parent.NewQty;
            Gravar = parent.NewGravar;
            Gravado = parent.NewGravado;
        }
        public static implicit operator NewFacturaItemViewModel(FacturadorViewModel vm) => new NewFacturaItemViewModel(vm);
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

        
        public static implicit operator Facturable(NewFacturaItemViewModel vm)=> vm.Item;
    }
}