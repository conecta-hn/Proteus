/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Collections.Generic;
using TheXDS.Proteus.Facturacion.Models.Base;

namespace TheXDS.Proteus.Facturacion.Models
{
    public class Producto : Facturable
    {
        public virtual List<InventarioConsumo> Consumos { get; set; } = new List<InventarioConsumo>();
    }
}