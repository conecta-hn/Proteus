/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Collections.Generic;
using System.Threading.Tasks;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.Plugins;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Proteus.Seeders
{
    /// <summary>
    /// Clase base para un <see cref="IAsyncDbSeeder"/> externo cargable como
    /// un <see cref="TheXDS.MCART.PluginSupport.Legacy.Plugin"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AsyncExternalSeeder<T> : ExternalSeeder, IAsyncDbSeeder where T : ModelBase, new()
    {
        /// <summary>
        /// Genera las nuevas entidades a insertar en la base de datos.
        /// </summary>
        /// <returns>
        /// Una enumeración de las nuevas entidades a insertar en la base de 
        /// datos.
        /// </returns>
        protected abstract IEnumerable<T> GenerateEntities();

        /// <summary>
        /// Obtiene el nombre a mostrar de este semillador.
        /// </summary>
        private string GetName => Name.OrNull() ?? GetType().NameOf() ?? GetType().Name;
        
        /// <summary>
        /// Ejecuta la acción de semillado de la base de datos de forma
        /// asíncrona.
        /// </summary>
        /// <param name="service">
        /// Servicio sobre el cual ejecutar las acciones de semillado.
        /// </param>
        /// <param name="reporter">
        /// Objeto que permite reportar el progreso de la operación.
        /// </param>
        /// <returns>
        /// El resultado detallado de la operación de semillado de la base de
        /// datos.
        /// </returns>
        public override sealed Task<DetailedResult> SeedAsync(IFullService service, IStatusReporter? reporter)
        {
            reporter?.UpdateStatus($"Creando {GetName}...");
            return service.AddAsync(GenerateEntities());
        }

        /// <summary>
        /// Realiza comprobaciones sobre la base de datos para determinar si
        /// debe ejecutarse o no este semillador.
        /// </summary>
        /// <param name="service">
        /// Servicio sobre el cual ejecutar las acciones de semillado.
        /// </param>
        /// <param name="reporter">
        /// Objeto que permite reportar el progreso de la operación.
        /// </param>
        /// <returns>
        /// <see langword="true"/> si debe ejecutarse el semillado en la base
        /// de datos, <see langword="false"/> en caso contrario.
        /// </returns>
        public override sealed async Task<bool> ShouldRunAsync(IReadAsyncService service, IStatusReporter? reporter)
        {
            var r = false;
            reporter?.UpdateStatus($"Comprobando {GetName}...");
            do
            {
                try
                {
                    r = !await service.AnyAsync<T>().Throwable();
                    break;
                }
                catch
                {
                    if (_tries++ > 3)
                    {
                        Proteus.MessageTarget?.Stop($"Hubo un problema al comprobar {GetName} y se ha superado la cantidad permitida de reintentos. Se debe ejecutar nuevamente el semillado manualmente más tarde.");
                        break;
                    }
                    Proteus.MessageTarget?.Warning($"Hubo un problema al comprobar {GetName}. Reintentando por {_tries}a vez en 3000 ms...");
                    await Task.Delay(3000);
                }
            } while (true);
            return r;
        }
        private byte _tries = 0;
    }
}