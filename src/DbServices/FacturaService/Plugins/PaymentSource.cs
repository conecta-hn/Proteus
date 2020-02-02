using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using TheXDS.MCART.Attributes;
using TheXDS.MCART.PluginSupport.Legacy;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Base;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.Plugins
{
    public abstract class PaymentSource : Plugin, IExposeGuid
    {
        private readonly IExposeGuid _implementor;

        /// <Inheritdoc/>
        public Guid Guid => _implementor.Guid;

        /// <summary>
        /// Inicializa una nueva instancia de la clase 
        /// <see cref="PaymentSource"/>.
        /// </summary>
        public PaymentSource()
        {
            _implementor = new ExposeGuidImplementor(this);
        }

        /// <summary>
        /// Crea un pago por medio de este <see cref="PaymentSource"/>.
        /// </summary>
        /// <param name="factTotal">Total a pagar de la factura.</param>
        /// <returns>
        /// Un nuevo <see cref="Payment"/> si el método de pago funcionará para
        /// el monto especificado, o <see langword="null"/> en caso contrario.
        /// </returns>
        public virtual Payment? MakePayment(decimal factTotal)
        {
            return new Payment
            {
                Amount = factTotal,
                Source = Guid,
                Timestamp = DateTime.Now
            };
        }
    }

    [Name("Efectivo"), Description("Permite realizar pagos en efectivo.")]
    [Guid("419d06c5-7a47-44d5-824b-bc573811084a")]
    public class EfectivoPaymentSource : PaymentSource
    {
    }

    [Name("Tarjeta"), Description("Permite realizar pagos con una tarjeta de crédito/débito.")]
    [Guid("5afe002f-27e4-46ef-89d6-c1a8c7cb0ca7")]
    public class TarjetaPaymentSource : PaymentSource
    {
    }

    [Name("Cortesía"), Description("Paga una factura a nombre de la empresa.")]
    [Guid("570695c9-786c-4e99-b46a-100a4d7be3d2")]
    public class CourtesyPaymentSource : PaymentSource
    {
    }
}
