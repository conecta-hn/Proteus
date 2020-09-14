using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.FacturacionUi.ViewModels
{
    /// <summary>
    /// ViewModel de Crud que define propiedades adicionales de edición del
    /// modelo <see cref="Factura"/>.
    /// </summary>
    public class FacturaCrudViewModel : ViewModel<Factura>
    {
        /// <summary>
        /// Obtiene el número de factura actual.
        /// </summary>
        public string? FacturaNumber => FacturaService.GetFactNum(Entity);
    }
}
