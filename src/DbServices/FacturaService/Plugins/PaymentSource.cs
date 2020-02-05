using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using TheXDS.MCART.Attributes;
using TheXDS.MCART.PluginSupport.Legacy;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Base;
using TheXDS.Proteus.Api;
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

    [Name("Pre-pagado"), Description("Paga una factura con fondos pre-pagados del cliente.")]
    [Guid("dabcf30e-3c02-44d3-9937-ab339c897b88")]
    public class PrepaidPaymentSource : PaymentSource
    {
        public override Task<Payment?> TryPayment(Factura fact, decimal amount)
        {
            if (fact?.Cliente is null) return Task.FromResult<Payment?>(null);
            if (fact.Cliente.Prepaid < amount) return Task.FromResult<Payment?>(null);
            fact.Cliente.Prepaid -= amount;
            return base.TryPayment(fact, amount);
        }
    }

    [Name("Crédito"), Description("Paga una factura por medio del crédito disponible del cliente.")]
    [Guid("4b919e69-0108-45d7-95eb-9b3c60c5ae93")]
    public class CreditPaymentSource : PaymentSource
    {
        public override Task<Payment?> TryPayment(Factura fact, decimal amount)
        {
            if (fact?.Cliente is null) return Task.FromResult<Payment?>(null);
            Proteus.Service<FacturaService>()!.Add(new FacturaXCobrar
            {
                Cliente = fact.Cliente,
                Parent = fact,
                Total = amount,
            });
            return base.TryPayment(fact, amount);
        }
    }
}
