/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Facturacion.Models
{
    public class Payment : TimestampModel<long>
    {
        public virtual Factura Parent { get; set; }
        public decimal Amount { get; set; }
        public override string ToString()
        {
            return $"{Amount:C} el {Timestamp}";
        }
    }
}