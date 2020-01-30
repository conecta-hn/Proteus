using System.Collections.Generic;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    /// <summary>
    /// Representa la categoría a la que un cliente puede pertenecer.
    /// </summary>
    public class ClienteCategory : Nameable<int>
    {
        /// <summary>
        /// Indica si los miembros de la categoría requieren RTN para facturar.
        /// </summary>
        public bool RequireRTN { get; set; }

        /// <summary>
        /// Colección de miembros de la categoría.
        /// </summary>
        public virtual List<Cliente> Members { get; set; } = new List<Cliente>();

        /// <summary>
        /// Colección de tabla de precios alternativos.
        /// </summary>
        public virtual TablaPrecio AltPrecios { get; set; } = null!;
    }
}