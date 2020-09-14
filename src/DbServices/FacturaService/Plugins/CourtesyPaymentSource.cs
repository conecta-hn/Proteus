using System;
using System.Runtime.InteropServices;
using TheXDS.MCART.Attributes;

namespace TheXDS.Proteus.Plugins
{
    [Name("Cortesía"), Description("Paga una factura a nombre de la empresa.")]
    [Guid("570695c9-786c-4e99-b46a-100a4d7be3d2")]
    public class CourtesyPaymentSource : PaymentSource
    {
    }
}
