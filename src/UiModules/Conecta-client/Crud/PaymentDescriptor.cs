/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Facturacion.Models;

namespace TheXDS.Proteus.Facturacion.Crud
{
    public class PaymentDescriptor : CrudDescriptor<Payment>
    {
        protected override void DescribeModel()
        {
            NumericProperty(p => p.Amount).Range(decimal.Zero, decimal.MaxValue).Important("Monto");
        }
    }
}