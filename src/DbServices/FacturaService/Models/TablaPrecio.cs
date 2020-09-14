using System.Collections.Generic;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    /// <summary>
    /// Describe una tabla de precios alternativos que pueden ser aplicados a
    /// una categoría de clientes.
    /// </summary>
    public class TablaPrecio : Nameable<int>
    {
        /// <summary>
        /// Colección de Ítems con precios alternativos definidos dentro de
        /// esta tabla.
        /// </summary>
        public virtual List<TablaPrecioItem> Items { get; set; } = new List<TablaPrecioItem>();

        /// <summary>
        /// Colección de categorías de cliente a las cuales se aplica esta
        /// tabla alternativa de precios.
        /// </summary>
        public virtual List<ClienteCategory> AppliedTo { get; set; } = new List<ClienteCategory>();
    }
}