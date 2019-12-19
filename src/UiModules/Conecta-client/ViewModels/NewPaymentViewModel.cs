/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Api;
using TheXDS.Proteus.Facturacion.Models;
using System.Linq;
using System.Windows;
using TheXDS.MCART.Types.Base;

namespace TheXDS.Proteus.Facturacion.ViewModels
{
    public class NewPaymentViewModel : NotifyPropertyChanged
    {
        private decimal _amount;
        private readonly FacturadorViewModel _parent;


        /// <summary>
        ///     Obtiene o establece el valor Amount.
        /// </summary>
        /// <value>El valor de Amount.</value>
        public decimal Amount
        {
            get => _amount;
            set
            {
                if (Change(ref _amount, value)) _parent.RefreshPayments();
            }
        }

        public NewPaymentViewModel(FacturadorViewModel parent)
        {
            _parent = parent;
        }
        public NewPaymentViewModel(FacturadorViewModel parent,Payment payment) : this(parent)
        {
            Amount = payment.Amount;
        }

        public static implicit operator NewPaymentViewModel(FacturadorViewModel parent) => new NewPaymentViewModel(parent);
    }
}