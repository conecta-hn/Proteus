using System.Linq;
using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.FacturacionUi.ViewModels
{
    /// <summary>
    /// ViewModel de Crud que define propiedades adicionales de edición del
    /// modelo <see cref="Paquete"/>.
    /// </summary>
    public class PaqueteCrudViewModel : ViewModel<Paquete>
    {
        private float _percent;
        private decimal _price;

        /// <summary>
        /// Porcentaje de descuento a aplicar respecto al precio original de 
        /// todos los ítems incluidos en el paquete.
        /// </summary>
        public float? Percent
        {
            get => _percent;
            set
            {
                if (Entity is null || value is null || !Change(ref _percent, value.Value)) return;
                _price = BulkPrice - BulkPrice * (decimal)(value / 100f);
                Entity.Precio = _price;
                Notify(nameof(Entity.Precio), nameof(Price));
            }
        }

        /// <summary>
        /// Precio de venta del paquete completo.
        /// </summary>
        public decimal? Price
        {
            get => _price;
            set
            {
                if (Entity is null || value is null || !Change(ref _price, value.Value)) return;
                Entity.Precio = _price;
                _percent = (float)((BulkPrice - _price) / BulkPrice) * 100f;
                Notify(nameof(Entity.Precio), nameof(Percent));

            }
        }

        /// <summary>
        /// Obtiene la suma de los precios de todos los ítems incluidos en el 
        /// paquete.
        /// </summary>
        public decimal BulkPrice => Entity?.Children.Sum(p => p.Precio) ?? 0m;
    }

}
