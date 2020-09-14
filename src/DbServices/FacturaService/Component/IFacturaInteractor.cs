using System.Collections.Generic;
using System.Threading.Tasks;

namespace TheXDS.Proteus.Component
{
    /// <summary>
    /// Describe una serie de miembros a implementar por un tipo que permita
    /// definir acciones y propiedades personalizadas para la facturación.
    /// </summary>
    public interface IFacturaInteractor
    {
        /// <summary>
        /// Obtiene una colección de columnas adicionales a presentar al
        /// imprimir una factura.
        /// </summary>
        IEnumerable<FacturaColumn> ExtraColumns
        {
            get
            {
                yield break;
            }
        }

        /// <summary>
        ///     Informa al <see cref="IFacturaInteractor"/> que la facturación 
        ///     se ha realizado.
        /// </summary>
        Task OnFacturateAsync() => Task.CompletedTask;
    }
}
