using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Models
{
    /// <summary>
    /// Ítem de una orden de trabajo.
    /// </summary>
    public class OrdenTrabajoItem : ModelBase<long>
    {
        /// <summary>
        /// Elemento facturable para el cual se ha creado este ítem.
        /// </summary>
        public virtual Facturable Item { get; set; }

        /// <summary>
        /// Orden de trabajo donde existe este ítem.
        /// </summary>
        public virtual OrdenTrabajo Parent { get; set; }

        /// <summary>
        /// Cantidad de ítems a procesar.
        /// </summary>
        public int Qty { get; set; }

        public override string ToString()
        {
            return $"{Qty} {Item?.Name}";
        }
    }
}