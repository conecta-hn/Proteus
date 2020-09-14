using System;
using System.Threading.Tasks;
using TheXDS.MCART.PluginSupport.Legacy;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Base;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.Plugins
{
    /// <summary>
    /// Plugin que define un componente de orígen de pago para una factura.
    /// </summary>
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
}
