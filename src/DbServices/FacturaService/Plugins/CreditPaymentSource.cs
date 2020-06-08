using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using TheXDS.MCART.Attributes;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.Plugins
{
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
