/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Threading.Tasks;
using TheXDS.Proteus.Component;

namespace TheXDS.Proteus.Api
{
    /// <summary>
    /// Define una serie de miembros a implementar por un tipo que permita
    /// agregar información inicial a una base de datos.
    /// </summary>
    public interface IAsyncDbSeeder
    {
        /// <summary>
        /// Ejecuta una operación de semillado sobre la base de datos.
        /// </summary>
        /// <param name="service">
        /// Servicio por medio del cual se realizará la operación.
        /// </param>
        /// <param name="reporter">
        /// Elemento de reporte de estado al usuario.
        /// </param>
        /// <returns>
        /// Una tarea que puede utilizarse para monitorear la operación
        /// asíncrona.
        /// </returns>
        Task<DetailedResult> SeedAsync(IFullService service, IStatusReporter reporter);

        /// <summary>
        /// Determina si es necesario ejecutar este semillador.
        /// </summary>
        /// <param name="service">
        /// Servicio por medio del cual se realizará la operación.
        /// </param>
        /// <param name="reporter">
        /// Elemento de reporte de estado al usuario.
        /// </param>
        /// <returns>
        /// <see langword="true"/> si es necesari ejecutar el semillador,
        /// <see langword="false"/> en caso contrario.
        /// </returns>
        Task<bool> ShouldRunAsync(IReadAsyncService service, IStatusReporter reporter);
    }
}