using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Conecta.Models;

namespace TheXDS.Proteus.Conecta.ViewModels
{
    /// <summary>
    /// Clase base personalizada para el ViewModel recompilado que se utilizará
    /// dentro del Crud generado para el modelo
    /// <see cref="Inversor"/>.
    /// </summary>
    public class InversorViewModel : ViewModel<Inversor>
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="InversorViewModel"/>.
        /// </summary>
        protected InversorViewModel()
        {
            RegisterPropertyChangeBroadcast(nameof(Entity.Inversion), nameof(Entity.InvTotal));
        }
    }
}