/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Collections.Generic;
using TheXDS.Proteus.Reporting;
using System;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Proteus.Component
{
    /// <summary>
    /// Clase base para crear filtros de búsqueda específicos para modelos 
    /// de datos.
    /// </summary>
    /// <typeparam name="T">
    /// Tipo implementado o derivado por el modelo para el cual esta
    /// instancia puede agregar un filtro.
    /// </typeparam>
    public abstract class ModelSearchFilter<T> : IModelSearchFilter
    {
        /// <summary>
        /// Configura los filtros para permitir búsquedas que incluyan
        /// entidades para las cuales esta instancia puede crearlos.
        /// </summary>
        /// <param name="mainFilter">
        /// Filtro principal de la consulta. Se traduce en concatenaciones
        /// de expresiones 
        /// <see cref="System.Linq.Enumerable.Where{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>.
        /// </param>
        /// <param name="orFilter">
        /// Filtro inclusivo de la consulta. Se traduce en una expresión
        /// concatenada de comparaciones "OR".
        /// </param>
        /// <param name="query">
        /// Cadena que describe la búsqueda solicitada por el usuario.
        /// </param>
        public abstract void AddFilter(ICollection<IFilter> mainFilter, ICollection<BinaryFilterBase> orFilter, string query);

        /// <summary>
        /// Comprueba si este <see cref="IModelSearchFilter"/> puede crear
        /// filtros para el modelo especificado.
        /// </summary>
        /// <param name="model">Modelo a comprobar.</param>
        /// <returns>
        /// <see langword="true"/> si esta instancia puede crear filtros de
        /// consulta para el modelo especificado, <see langword="false"/>
        /// en caso contrario.
        /// </returns>
        public bool UsableFor(Type model) => model.Implements<T>();
    }
}