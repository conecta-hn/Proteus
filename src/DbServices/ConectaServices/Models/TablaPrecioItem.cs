/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.Facturacion.Models.Base;

namespace TheXDS.Proteus.Facturacion.Models
{
    public class TablaPrecioItem : Valuable<int>
    {
        public virtual TablaPrecio Parent { get; set; }
        public virtual Facturable Item { get; set; }        
    }
}
