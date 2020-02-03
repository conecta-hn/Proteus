using System.Collections.Generic;
using System.Windows;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.FacturacionUi.ViewModels;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.FacturacionUi.Component
{
    /// <summary>
    ///     Define una serie de miembros a implementar por un tipo que
    ///     interactúe con el subsistema de facturación de este módulo.
    /// </summary>
    public interface IFacturaUIInteractor : IFacturaInteractor
    {
        /// <summary>
        ///     Obtiene un elemento de UI adicional a integrar en la UI de la
        ///     ventana de facturación.
        /// </summary>
        FrameworkElement ExtraUi { get; }

        /// <summary>
        ///     Obtiene un valor que indica si es posibloe interactuar con el
        ///     bloque de UI adicional.
        /// </summary>
        bool CanInteract { get; }

        /// <summary>
        ///     Obtiene un elemento de UI adicional a integrar en la ventana de
        ///     detalles para el cliente.
        /// </summary>
        FrameworkElement ExtraDetails { get; }

        /// <summary>
        ///     Obtiene o establece el elemento padre de este 
        ///     <see cref="IFacturaUIInteractor"/>.
        /// </summary>
        FacturadorViewModel VmParent { get; set; }

        /// <summary>
        ///     Comprueba si el ítem puede ser agregado a la factura.
        /// </summary>
        /// <param name="item">Ítem a comprobar.</param>
        /// <returns>
        ///     <see langword="true"/> si el ítem puede ser agregado a la
        ///     factura, <see langword="false"/> en caso contrario.
        /// </returns>
        bool CanAddItem(NewFacturaItemViewModel item);

        /// <summary>
        ///     Comprueba si es posible facturar.
        /// </summary>
        /// <returns>
        ///     <see langword="true"/> si es posible facturar,
        ///     <see langword="false"/> en caso contrario.
        /// </returns>
        bool CanFacturate();

        /// <summary>
        ///     Obtiene un valor que indica si es posible seleccionar al
        ///     cliente especificado.
        /// </summary>
        /// <param name="cliente">
        ///     Cliente a comprobar.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> si es posible seleccionar al cliente,
        ///     <see langword="false"/> en caso contrario.
        /// </returns>
        bool CanSelectCliente(Cliente cliente);

        /// <summary>
        /// Permite definir acciones especiales a ejecutar luego de seleccionar
        /// a un cliente.
        /// </summary>
        void OnClienteSelected();

        /// <summary>
        ///     Genera pagos adicionales de forma automática.
        /// </summary>
        /// <returns>
        ///     Una colección de pagos que se pueden utilizar en esta factura.
        /// </returns>
        IEnumerable<NewPaymentViewModel> GeneratePayments();
    }
}
