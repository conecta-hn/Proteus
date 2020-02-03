﻿using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.FacturacionUi.ViewModels
{
    /// <summary>
    /// ViewModel de Crud que define propiedades adicionales de edición del
    /// modelo <see cref="ItemFactura"/>.
    /// </summary>
    public class ItemFacturaCrudViewModel : ViewModel<ItemFactura>
    {
        private Facturable _actualItem = null!;

        /// <summary>
        ///     Obtiene o establece el valor ActualItem.
        /// </summary>
        /// <value>El valor de ActualItem.</value>
        public Facturable ActualItem
        {
            get => _actualItem;
            set
            {
                if (Change(ref _actualItem, value))
                {
                    Entity.Item = value;
                    Precio = value?.Precio ?? 0m;
                    Isv = (value?.Isv * 100f) ?? 0f;
                }
            }
        }

        private decimal _precio;

        /// <summary>
        ///     Obtiene o establece el valor Precio.
        /// </summary>
        /// <value>El valor de Precio.</value>
        public decimal Precio
        {
            get => _precio;
            set
            {
                if (Change(ref _precio, value)) Entity.StaticPrecio = value;
            }
        }

        /// <summary>
        ///     Obtiene o establece el valor Precio.
        /// </summary>
        /// <value>El valor de Precio.</value>
        public decimal Descuento
        {
            get => _descuento;
            set
            {
                if (Change(ref _descuento, value)) Entity.StaticDescuento = value;
            }
        }

        private float? _isv;
        private decimal _descuento;

        /// <summary>
        ///     Obtiene o establece el valor Isv.
        /// </summary>
        /// <value>El valor de Isv.</value>
        public float Isv
        {
            get => _isv ?? 0f;
            set
            {
                float? v = value > 0f ? (float?)value : null;
                if (Change(ref _isv, v)) Entity.StaticIsv = v;
            }
        }
    }

}
