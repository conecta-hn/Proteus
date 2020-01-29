/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Collections.Generic;
using TheXDS.Proteus.Reporting;
using System;

namespace TheXDS.Proteus.Component
{
    /// <summary>
    /// Define una serie de miembros a implementar por un tipo que permita
    /// crear filtros de búsqueda para realizar consultas a una base de
    /// datos por medio de Linq-to-SQL.
    /// </summary>
    public interface IModelSearchFilter
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
        void AddFilter(ICollection<IFilter> mainFilter, ICollection<BinaryFilterBase> orFilter, string query);
        
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
        bool UsableFor(Type model);
    }


}