using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using TheXDS.MCART.Attributes;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.Plugins
{
    /// <summary>
    /// Método de pago que admite dejar un saldo pendiente a ser cancelado al
    /// crédito.
    /// </summary>
    [Name("Crédito"), Description("Paga una factura por medio del crédito disponible del cliente.")]
    [Guid("4b919e69-0108-45d7-95eb-9b3c60c5ae93")]
    public class CreditPaymentSource : PaymentSource
    {
        public override Task<Payment?> TryPayment(Factura fact, decimal amount)
        {
            if (!(fact is {Cliente: Cliente c })) return Task.FromResult<Payment?>(null);
            if (!c.CanCredit || IsCreditLimitReached(c,amount)) return Task.FromResult<Payment?>(null);
            
            Proteus.Service<FacturaService>()!.Add(new FacturaXCobrar
            {
                Cliente = fact!.Cliente!,
                Parent = fact,
                Total = amount,
            });
            return base.TryPayment(fact, amount);
        }

        private bool IsCreditLimitReached(Cliente c, decimal amount)
        {
            return c.AvailableCredit is { } v && amount > v;
        }
    }
}
