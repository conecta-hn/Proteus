using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
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
        /// Intenta efectuar un pago por medio de este <see cref="PaymentSource"/>.
        /// </summary>
        /// <param name="fact">Referencia a la factura.</param>
        /// <returns>
        /// <see langword="true"/> si el pago pudo ser procesado correctamente,
        /// <see langword="false"/> en caso contrario.
        /// </returns>
        public virtual Task<Payment?> TryPayment(Factura fact, decimal amount)
        {
            return Task.FromResult<Payment?>(new Payment
            {
                Source = Guid,
                Amount = amount,
                Timestamp = DateTime.Now
            });
        }
    }

    [Name("Efectivo"), Description("Permite realizar pagos en efectivo.")]
    [Guid("419d06c5-7a47-44d5-824b-bc573811084a")]
    public class EfectivoPaymentSource : PaymentSource
    {
    }
}
