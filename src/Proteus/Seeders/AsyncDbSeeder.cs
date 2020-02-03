/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Collections.Generic;
using System.Threading.Tasks;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.Models.Base;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Proteus.Seeders
{
    /// <summary>
    /// Clase base para todos los <see cref="IAsyncDbSeeder"/> que puedan 
    /// semillar bases de datos basados en un modelo concreto.
    /// </summary>
    /// <typeparam name="T">Modelo de datos a semillar.</typeparam>
    public abstract class AsyncDbSeeder<T> : IAsyncDbSeeder where T: ModelBase, new()
    {
        /// <summary>
        /// Nombre de este semillador de datos.
        /// </summary>
        protected virtual string? Name { get; }

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
        private string GetName => Name.OrNull() ?? typeof(T).NameOf() ?? GetType().NameOf() ?? GetType().Name;

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
        public Task<DetailedResult> SeedAsync(IFullService service, IStatusReporter? reporter)
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
        public virtual async Task<bool> ShouldRunAsync(IReadAsyncService service, IStatusReporter? reporter)
        {
            reporter?.UpdateStatus($"Comprobando {GetName}...");
            return !await service.AnyAsync<T>();
        }
    }
}