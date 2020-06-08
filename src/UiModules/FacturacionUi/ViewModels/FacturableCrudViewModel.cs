using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.FacturacionUi.ViewModels
{
    /// <summary>
    /// Clase base personalizada para el ViewModel recompilado que se utilizará
    /// dentro del Crud generado para el modelo
    /// <see cref="Facturable"/>.
    /// </summary>
    public abstract class FacturableCrudViewModel<T> : ViewModel<T> where T : Facturable
    {
        /// <summary>
        ///     Obtiene o establece el precio con ISV incluido.
        /// </summary>
        /// <value>El valor de PrecioIsv.</value>
        public decimal PrecioIsv
        {
            get
            {
                decimal m;
                return (m = IsvMultUp) <= 0m ? 0m : (Entity?.Precio ?? 0m) * m;
            }
            set
            {
                if (Entity is null) return;
                Entity.Precio = value / IsvMultUp;
                Notify(nameof(Entity.Precio), nameof(PrecioIsv));
            }
        }

        private decimal IsvMultDown => (decimal)((100f - (Entity?.Isv ?? 0f)) / 100f);
        private decimal IsvMultUp => (decimal)((100f + (Entity?.Isv ?? 0f)) / 100f);

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="ProductoCrudViewModel"/>.
        /// </summary>
        protected FacturableCrudViewModel()
        {
            RegisterPropertyChangeBroadcast(
                nameof(PrecioIsv),
                nameof(Entity),
                nameof(Entity.Precio), "Entity.Precio",
                nameof(Entity.Isv), "Entity.Isv");
        }
    }
}