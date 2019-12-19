/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Facturacion.Models;
using System.Linq;
using TheXDS.MCART.ViewModel;

namespace TheXDS.Proteus.Facturacion.ViewModels
{
    public abstract class PaqueteCrudViewModel : DynamicViewModel<Paquete>
    {
        private float _percent;
        private decimal _price;

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

        public decimal BulkPrice => Entity?.Children.Sum(p => p.Precio) ?? 0m;
    }
}