/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Api;
using TheXDS.Proteus.Component;
using System.Threading.Tasks;
using TheXDS.MCART.PluginSupport.Legacy;

namespace TheXDS.Proteus.Plugins
{
    /// <summary>
    ///     Clase base para todos los <see cref="Plugin"/> que implementan 
    ///     <see cref="IAsyncDbSeeder"/>.
    /// </summary>
    public abstract class ExternalSeeder : Plugin, IAsyncDbSeeder
    {
        /// <summary>
        ///     Ejecuta una operación de semillado sobre la base de datos.
        /// </summary>
        /// <param name="service">
        ///     Servicio por medio del cual se realizará la operación.
        /// </param>
        /// <param name="reporter">
        ///     Elemento de reporte de estado al usuario.
        /// </param>
        /// <returns>
        ///     Una tarea que puede utilizarse para monitorear la operación
        ///     asíncrona.
        /// </returns>
        public abstract Task<DetailedResult> SeedAsync(IFullService service, IStatusReporter reporter);

        /// <summary>
        ///     Determina si es necesario ejecutar este semillador.
        /// </summary>
        /// <param name="service">
        ///     Servicio por medio del cual se realizará la operación.
        /// </param>
        /// <param name="reporter">
        ///     Elemento de reporte de estado al usuario.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> si es necesari ejecutar el semillador,
        ///     <see langword="false"/> en caso contrario.
        /// </returns>
        public abstract Task<bool> ShouldRunAsync(IReadAsyncService service, IStatusReporter reporter);
    }
}
