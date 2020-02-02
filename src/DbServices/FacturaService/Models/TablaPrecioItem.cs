using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    /// <summary>
    /// Describe a un ítem en una tabla de precios alternativos.
    /// </summary>
    public class TablaPrecioItem : Valuable<int>
    {
        /// <summary>
        /// Ítem al cual se aplicarán los nuevos precios.
        /// </summary>
        public virtual Facturable Item { get; set; } = null!;
    }
}