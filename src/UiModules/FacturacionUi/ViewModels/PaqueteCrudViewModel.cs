using System;
using System.Linq;
using TheXDS.MCART;
using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Crud.Base;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.FacturacionUi.ViewModels
{

    /// <summary>
    /// Clase base personalizada para el ViewModel recompilado que se utilizará
    /// dentro del Crud generado para el modelo
    /// <see cref="OrdenTrabajo"/>.
    /// </summary>
    public class OrdenTrabajoViewModel : ViewModel<OrdenTrabajo>
    {
        public float DescuentoPercent
        {
            get
            {
                var tot = 0m;
                var exonerar = Entity.Cliente?.Exoneraciones.Any(p => DateTime.Today.IsBetween(p.Timestamp, p.Void)) ?? false;
                foreach (var j in Entity.Items)
                {
                    var precio = j.Item.Precio;
                    if (!exonerar)
                        precio += (j.Item.Precio * (decimal)((j.Item.Isv / 100f) ?? 0f));
                    tot += precio * j.Qty;
                }
                return tot == 0m ? 0f:(float)(Entity.Descuentos / tot);
            }

            set
            {
                var tot = 0m;
                var exonerar = Entity.Cliente?.Exoneraciones.Any(p => DateTime.Today.IsBetween(p.Timestamp, p.Void)) ?? false;
                foreach (var j in Entity.Items)
                {
                    var precio = j.Item.Precio;
                    if (!exonerar)
                        precio += (j.Item.Precio * (decimal)((j.Item.Isv / 100f) ?? 0f));
                    tot += precio * j.Qty;
                }

                Entity.Descuentos = tot * (decimal)value / 100m;
            }
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="OrdenTrabajoViewModel"/>.
        /// </summary>
        public OrdenTrabajoViewModel()
        {
            // TODO: Registrar propiedades enlazadas con los métodos RegisterPropertyChangeBroadcast() o RegisterPropertyChangeTrigger().
        }
    }

    /// <summary>
    /// ViewModel de Crud que define propiedades adicionales de edición del
    /// modelo <see cref="Paquete"/>.
    /// </summary>
    public class PaqueteCrudViewModel : FacturableCrudViewModel<Paquete>
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
                if (Entity is null || value is null || !Change(ref _price, value.Value * (decimal)(100f - (Entity!.Isv ?? 0f)))) return;
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

    /// <summary>
    /// Clase base personalizada para el ViewModel recompilado que se utilizará
    /// dentro del Crud generado para el modelo
    /// <see cref="Facturable"/>.
    /// </summary>
    public abstract class FacturableCrudViewModel<T> : ViewModel<T> where T:Facturable
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

    /// <summary>
    /// Clase base personalizada para el ViewModel recompilado que se utilizará
    /// dentro del Crud generado para el modelo
    /// <see cref="Producto"/>.
    /// </summary>
    public class ProductoCrudViewModel : FacturableCrudViewModel<Producto>
    {
    }

    /// <summary>
    /// Clase base personalizada para el ViewModel recompilado que se utilizará
    /// dentro del Crud generado para el modelo
    /// <see cref="Servicio"/>.
    /// </summary>
    public class ServicioCrudViewModel : FacturableCrudViewModel<Servicio>
    {
    }
}