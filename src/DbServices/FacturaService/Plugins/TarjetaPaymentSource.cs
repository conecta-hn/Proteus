using System;
using System.Runtime.InteropServices;
using TheXDS.MCART.Attributes;

namespace TheXDS.Proteus.Plugins
{
    /// <summary>
    /// Método de pago integrado que permite realizar pagos con tarjeta de
    /// crédito/débito de cualquier banco. Este método no se integra con ningún
    /// servicio bancario, por lo que únicamente se utiliza para registrar el
    /// monto cancelado por medio de una tarjeta, y no para efectuar el cobro
    /// correspondiente.
    /// </summary>
    [Name("Tarjeta"), Description("Permite realizar pagos con una tarjeta de crédito/débito.")]
    [Guid("5afe002f-27e4-46ef-89d6-c1a8c7cb0ca7")]
    public class TarjetaPaymentSource : PaymentSource
    {
    }
}
