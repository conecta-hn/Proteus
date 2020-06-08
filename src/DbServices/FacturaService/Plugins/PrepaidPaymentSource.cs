using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using TheXDS.MCART.Attributes;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.Plugins
{
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
}
