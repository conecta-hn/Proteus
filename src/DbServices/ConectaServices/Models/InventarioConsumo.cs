/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Facturacion.Models
{
    public class InventarioConsumo : ModelBase<int>
    {
        public int InventarioItemId { get; set; }
        public int Qty { get; set; } = 1;
    }
}