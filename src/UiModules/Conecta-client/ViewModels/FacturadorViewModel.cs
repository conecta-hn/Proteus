﻿/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Component;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Crud;
using TheXDS.Proteus.Facturacion.Component;
using TheXDS.Proteus.Facturacion.Models;
using TheXDS.Proteus.Facturacion.Models.Base;
using TheXDS.Proteus.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Extensions;
using TheXDS.MCART.ViewModel;

namespace TheXDS.Proteus.Facturacion.ViewModels
{
    public class FacturadorViewModel : ServicingPageViewModel<ConectaService>
    {
        private bool _closeAfterFacturate = false;
        private readonly IFacturaUIInteractor _interactor;
        private string _newItemCode;
        private Facturable _newItem;
        private int _newQty;
        private float _newGravado;
        private bool _gravar;
        private Factura _currentFactura;
        private decimal _descuento;
        private decimal _otrosCargos;
        private NewPaymentViewModel _selectedPayment;
        private bool _printFactura = true;

        public ObservableListWrap<Facturable> Facturables { get; private set; }
        public ObservableListWrap<Cliente> Clientes { get; private set; }
        public ObservableCollection<NewFacturaItemViewModel> NewItems { get; } = new ObservableCollection<NewFacturaItemViewModel>();
        public ObservableCollection<NewPaymentViewModel> NewPayments { get; } = new ObservableCollection<NewPaymentViewModel>();

        public decimal SubTotal => NewItems.Sum(p => p.SubTotal);
        public decimal SubTGravable => NewItems.Sum(p => p.MontoGravado);
        public decimal SubTGravado => NewItems.Sum(p => p.SubTGrav);
        public decimal SubTDescuentos => NewItems.Sum(p => p.DescuentosOtorgados);
        public decimal SubTFinal => NewItems.Sum(p => p.FinalSubT);

        /// <summary>
        ///     Obtiene o establece el valor Descuento.
        /// </summary>
        /// <value>El valor de Descuento.</value>
        public decimal Descuento
        {
            get => _descuento;
            set => Change(ref _descuento, value);
        }

        /// <summary>
        ///     Obtiene o establece el valor OtrosCargos.
        /// </summary>
        /// <value>El valor de OtrosCargos.</value>
        public decimal OtrosCargos
        {
            get => _otrosCargos;
            set => Change(ref _otrosCargos, value);
        }

        /// <summary>
        ///     Obtiene el valor total de la factura.
        /// </summary>
        public decimal Total => SubTFinal - Descuento + OtrosCargos;

        public decimal Paid => NewPayments.Sum(p => p.Amount);
        public decimal Vuelto => Total - Paid;

        public string VueltoLabel => Vuelto > 0 ? "Restante" : "Cambio";

        /// <summary>
        ///     Obtiene o establece el valor NewItemCode.
        /// </summary>
        /// <returns>El valor de NewItemCode.</returns>
        public string NewItemCode
        {
            get => _newItemCode;
            set
            {
                if (Change(ref _newItemCode, value))
                {
                    NewItemChanged(_newItem = Facturables.FirstOrDefault(p => p.Id.ToString() == value));
                    Notify(nameof(NewItem));
                }
            }
        }

        /// <summary>
        ///     Obtiene o establece el valor NewItem.
        /// </summary>
        /// <returns>El valor de NewItem.</returns>
        public Facturable NewItem
        {
            get => _newItem;
            set
            {
                if (Change(ref _newItem, value))
                {
                    _newItemCode = value?.Id.ToString();
                    NewItemChanged(value);
                    Notify(nameof(NewItemCode));
                }
            }
        }


        private bool _canDescuento;

        /// <summary>
        ///     Obtiene o establece el valor CanDescuento.
        /// </summary>
        /// <value>El valor de CanDescuento.</value>
        public bool CanDescuento
        {
            get => _canDescuento;
            set => Change(ref _canDescuento, value);
        }


        /// <summary>
        ///     Obtiene el comando relacionado a la acción Elevate.
        /// </summary>
        /// <returns>El comando Elevate.</returns>
        public SimpleCommand ElevateCommand { get; }

        private void OnElevate()
        {
            CanDescuento = !CanDescuento;
        }
        
        private void NewItemChanged(Facturable value)
        {
            NewQty = value is null ? 0 : 1;
            NewGravado = value?.Isv ?? 0f;
            NewGravar = value?.Isv.HasValue ?? false;
        }

        /// <summary>
        ///     Obtiene el precio del nuevo item a facturar.
        /// </summary>
        public decimal NewPrecio => NewItem?.Precio ?? 0m;

        /// <summary>
        ///     Obtiene o establece el valor NewQty.
        /// </summary>
        /// <value>El valor de NewQty.</value>
        public int NewQty
        {
            get => _newQty;
            set => Change(ref _newQty, value);
        }

        /// <summary>
        ///     Obtiene o establece el valor Gravable.
        /// </summary>
        /// <value>El valor de Gravable.</value>
        public bool NewGravable => NewItem?.Isv.HasValue ?? false;

        /// <summary>
        ///     Obtiene o establece el valor Gravar.
        /// </summary>
        /// <value>El valor de Gravar.</value>
        public bool NewGravar
        {
            get => _gravar;
            set => Change(ref _gravar, value);
        }

        /// <summary>
        ///     Obtiene o establece el valor NewGravado.
        /// </summary>
        /// <value>El valor de NewGravado.</value>
        public float NewGravado
        {
            get => _newGravado;
            set => Change(ref _newGravado, value);
        }

        /// <summary>
        ///     Obtiene o establece el valor CurrentFactura.
        /// </summary>
        /// <value>El valor de CurrentFactura.</value>
        public Factura CurrentFactura
        {
            get => _currentFactura;
            set => Change(ref _currentFactura, value);
        }

        public Visibility IsNewV => (CurrentFactura?.IsNew ?? false)? Visibility.Visible : Visibility.Collapsed;
        public Visibility NotNewV => (CurrentFactura?.IsNew ?? true)? Visibility.Collapsed : Visibility.Visible;
        public string FacturaNumber => ConectaService.GetFactNum(CurrentFactura);

        public FacturadorViewModel(ICloseable host) : this(host, (IFacturaUIInteractor)null)
        {
        }

        public FacturadorViewModel(ICloseable host, IFacturaUIInteractor interactor) : this(host, interactor, null)
        {
        }

        public FacturadorViewModel(ICloseable host, Factura factura) : base(host)
        {
            RegisterPropertyChangeBroadcast(nameof(CurrentFactura), nameof(IsNewV), nameof(NotNewV), nameof(FacturaNumber));
            RegisterPropertyChangeBroadcast(nameof(NewItem), nameof(NewPrecio), nameof(NewGravable));
            RegisterPropertyChangeBroadcast(nameof(NewCliente), nameof(IsClienteSelected), nameof(IsClienteNotSelected));
            RegisterPropertyChangeTrigger(nameof(SubTFinal), nameof(SubTotal),nameof(SubTGravado), nameof(SubTDescuentos));
            RegisterPropertyChangeTrigger(nameof(Total), nameof(SubTFinal), nameof(Descuento), nameof(OtrosCargos));
            RegisterPropertyChangeBroadcast(nameof(PrintFactura), nameof(FacturarBtnTitle));
            AddNewCommand = new SimpleCommand(OnAddNew);
            FacturarCommand = new SimpleCommand(OnFacturar, false);
            AddPaymentCommand = new SimpleCommand(OnAddPayment);
            RemovePaymentCommand = new SimpleCommand(OnRemovePayment);
            ElevateCommand = new SimpleCommand(OnElevate);
            NewClienteCommand = new SimpleCommand(OnNewCliente);
            OkClienteCommand = new SimpleCommand(OnOkCliente);
            CancelClienteCommand = new SimpleCommand(OnCancelCliente);
            SaveAsCotizCommand = new SimpleCommand(OnSaveAsCotiz);
            Title = "Facturación";
            if (factura is null) NewFactura();
            else SetFactura(factura);
        }

        private void SetFactura(Factura f)
        {
            _closeAfterFacturate = true;
            if (f is null) throw new ArgumentNullException(nameof(f));
            CurrentFactura = f;
            ClearNew();
            NewItems.Clear();
            NewItems.AddRange(f.Items.Select(p => new NewFacturaItemViewModel(this, p)));
            NewPayments.Clear();
            NewPayments.AddRange(f.Payments.Select(p => new NewPaymentViewModel(this, p)));
            if (_interactor?.CanSelectCliente(f.Cliente) ?? true)
            {
                NewCliente = f.Cliente;
                _interactor?.OnClienteSelected();
            }
            Descuento = f.Descuentos;
            OtrosCargos = f.OtrosCargos;
            PrintFactura = true;            
            CheckCanFacturate();
        }

        public FacturadorViewModel(ICloseable host, IFacturaUIInteractor interactor, Factura factura) : this(host, factura)
        {
            if (!(interactor is null))
            {
                _interactor = interactor;
                _interactor.VmParent = this;
            }
        }

        private Cliente _newCliente;
        private bool _isEditingCliente;

        /// <summary>
        ///     Obtiene o establece el valor NewCliente.
        /// </summary>
        /// <value>El valor de NewCliente.</value>
        public Cliente NewCliente
        {
            get => _newCliente;
            set
            {
                if (!_interactor?.CanSelectCliente(value) ?? false) return;
                if (Change(ref _newCliente, value))
                {
                    Notify(nameof(NewClienteBtnLabel));
                    ClienteEditor.ViewModel.Entity = value;
                    _interactor?.OnClienteSelected();
                    //TODO: agregar descuentos
                }
            }
        }

        public string NewClienteBtnLabel => NewCliente is null ? "+" : "✎";

        private void OnNewCliente()
        {
            NewCliente ??= new Cliente()
            {
                Timestamp = DateTime.Now
            };
            OnEditCliente();
        }
        private void OnEditCliente()
        {
            IsEditingCliente = true;
        }
        private void OnOkCliente()
        {
            IsEditingCliente = false;
            if (!Clientes.Contains(NewCliente))
            {
                var c = NewCliente;
                Clientes.Add(c);
                NewCliente = c;
            }
            else
            {
                Clientes.RefreshItem(NewCliente);
            }
        }
        private void OnCancelCliente()
        {
            if (!(NewCliente is null))
            {
                if (NewCliente.IsNew) NewCliente = null;
                else if (NewCliente.ChangesPending()) NewCliente.Rollback();
            }
            IsEditingCliente = false;
            Notify(nameof(NewCliente));
        }

        protected override Task OnStartup()
        {
            Clientes = GetObservable<Cliente>();
            Facturables = GetObservable<Facturable>();
            return Task.CompletedTask;
        }

        public CrudElement ClienteEditor { get; } = CrudElement.ForModel<Cliente>();

        public bool IsEditingCliente
        {
            get => _isEditingCliente;
            set => Change(ref _isEditingCliente, value);
        }
       
        /// <summary>
        ///     Obtiene el comando relacionado a la acción AddNew.
        /// </summary>
        /// <returns>El comando AddNew.</returns>
        public SimpleCommand AddNewCommand { get; }

        /// <summary>
        ///     Obtiene el comando relacionado a la acción Facturar.
        /// </summary>
        /// <returns>El comando Facturar.</returns>
        public SimpleCommand FacturarCommand { get; }

        /// <summary>
        ///     Obtiene el comando relacionado a la acción AddPayment.
        /// </summary>
        /// <returns>El comando AddPayment.</returns>
        public SimpleCommand AddPaymentCommand { get; }

        /// <summary>
        ///     Obtiene el comando relacionado a la acción RemovePayment.
        /// </summary>
        /// <returns>El comando RemovePatment.</returns>
        public SimpleCommand RemovePaymentCommand { get; }

        public SimpleCommand NewClienteCommand { get; }
        public SimpleCommand OkClienteCommand { get; }
        public SimpleCommand CancelClienteCommand { get; }


        /// <summary>
        ///     Obtiene el comando relacionado a la acción OnSaveAsCotiz.
        /// </summary>
        /// <returns>El comando SaveAsCOtiz.</returns>
        public SimpleCommand SaveAsCotizCommand { get; }

        private async void OnSaveAsCotiz()
        {
            foreach (var j in NewItems)
                CurrentFactura.Items.Add(j);
            CurrentFactura.Cliente = NewCliente;

            await BusyOp(() => Service.AddAsync((Cotizacion)CurrentFactura));
            NewFactura();
        }

        /// <summary>
        ///     Obtiene o establece el valor SelectedPayment.
        /// </summary>
        /// <value>El valor de SelectedPayment.</value>
        public NewPaymentViewModel SelectedPayment
        {
            get => _selectedPayment;
            set => Change(ref _selectedPayment, value);
        }        

        public Visibility HasExtraUi => _interactor?.ExtraUi is null ? Visibility.Collapsed : Visibility.Visible;
        public bool CanExtraUiInteract => _interactor?.CanInteract ?? false;
        public Visibility HasExtraDetails => _interactor?.ExtraDetails is null ? Visibility.Collapsed : Visibility.Visible;

        public Visibility IsClienteSelected => NewCliente is null ? Visibility.Collapsed : Visibility.Visible;
        public Visibility IsClienteNotSelected => NewCliente is null ? Visibility.Visible : Visibility.Collapsed;

        public FrameworkElement ExtraUi => _interactor?.ExtraUi;
        public FrameworkElement ExtraDetails => _interactor?.ExtraDetails;

        private void OnRemovePayment()
        {
            NewPayments.Remove(SelectedPayment);
            SelectedPayment = null;
        }

        private void OnAddPayment()
        {
            SelectedPayment = NewPayments.Push(new NewPaymentViewModel(this));
        }

        private async void OnFacturar()
        {
            void Cleanup()
            {
                IsBusy = false;
                CurrentFactura.Payments.Clear();
                CurrentFactura.Items.Clear();
                CurrentFactura.Cliente = null;
            }

            if (!CurrentFactura.IsNew) return;
            
            if (CurrentFactura.Total >= 10000m && (NewCliente?.Rtn?.IsEmpty() ?? true))
            {
                Proteus.MessageTarget?.Stop("Esta factura requiere RTN del cliente.");
                return;
            }

            IsBusy = true;

            foreach (var j in NewItems)
                CurrentFactura.Items.Add(j);

            CurrentFactura.Cliente = NewCliente;

            ConectaService.AddFactura(CurrentFactura, PrintFactura, _interactor);

            await Proteus.Service<ConectaService>().SaveAsync();
            await (_interactor?.OnFacturate() ?? Task.CompletedTask);

            if (_closeAfterFacturate)
            {
                Close();
            }
            else
            {
                NewFactura();
            }
            IsBusy = false;
        }

        /// <summary>
        ///     Obtiene o establece el valor PrintFactura.
        /// </summary>
        /// <value>El valor de PrintFactura.</value>
        public bool PrintFactura
        {
            get => _printFactura;
            set => Change(ref _printFactura, value);
        }

        public string FacturarBtnTitle => PrintFactura ? "Facturar" : "Facturar como proforma";

        private void OnAddNew()
        {
            if (NewItems.Any(p=>p.Item == NewItem))
            {
                Proteus.MessageTarget?.Stop($"El ìtem {NewItem.Id} ya ha sido agregado a la factura.");
                return;
            }
            NewItems.Add(this);
            ClearNew();
            RefreshSubtotals();
        }

        private void ClearNew()
        {
            NewItem = null;
        }

        private void NewFactura()
        {
            CurrentFactura = new Factura();
            ClearNew();
            NewItems.Clear();
            NewPayments.Clear();
            NewCliente = null;
            Descuento = 0m;
            OtrosCargos = 0m;
            PrintFactura = true;
            CheckCanFacturate();
        }

        public void RefreshSubtotals()
        {
            Notify(
                nameof(SubTotal),
                nameof(SubTGravable),
                nameof(SubTGravado),
                nameof(SubTDescuentos),
                nameof(SubTFinal)
                );
            CheckCanFacturate();
        }

        public void RefreshPayments()
        {
            Notify(
                nameof(Paid),
                nameof(Vuelto),
                nameof(VueltoLabel)
                );
            CheckCanFacturate();
        }

        private void CheckCanFacturate()
        {
            FacturarCommand.SetCanExecute(
                !(CurrentFactura is null)
                && !(NewCliente is null)
                && NewItems.Any()
                && Paid >= Total
                && (_interactor?.CanFacturate() ?? true));
        }
    }
}