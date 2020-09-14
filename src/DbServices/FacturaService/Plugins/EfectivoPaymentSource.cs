using System;
using System.Runtime.InteropServices;
using TheXDS.MCART.Attributes;

namespace TheXDS.Proteus.Plugins
{
    /// <summary>
    /// Método de pago integrado que permite pagar una factura en efectivo.
    /// </summary>
    [Name("Efectivo"), Description("Permite realizar pagos en efectivo.")]
    [Guid("419d06c5-7a47-44d5-824b-bc573811084a")]
    public class EfectivoPaymentSource : PaymentSource
    {
    }
}