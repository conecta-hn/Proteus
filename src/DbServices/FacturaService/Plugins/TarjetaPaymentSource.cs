using System;
using System.Runtime.InteropServices;
using TheXDS.MCART.Attributes;

namespace TheXDS.Proteus.Plugins
{
    [Name("Tarjeta"), Description("Permite realizar pagos con una tarjeta de crédito/débito.")]
    [Guid("5afe002f-27e4-46ef-89d6-c1a8c7cb0ca7")]
    public class TarjetaPaymentSource : PaymentSource
    {
    }
}
