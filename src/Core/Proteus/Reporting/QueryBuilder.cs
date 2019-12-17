/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace TheXDS.Proteus.Reporting
{
    /// <summary>
    ///     Contiene métodos para construir Queries a partir de una lista de 
    ///     <see cref="IFilter"/> especificados.
    /// </summary>
    public static class QueryBuilder
    {
        /// <summary>
        ///     Construye un Query a partir de los filtros especificados.
        /// </summary>
        /// <param name="model">Modelo para el cual construir un Query.</param>
        /// <param name="filters">
        ///     Secuencia de filtros a aplicar al Query.
        ///     </param>
        /// <returns>
        ///     Un Query con todos los filtros especificados aplicados.
        /// </returns>
        public static IQueryable BuildQuery(Type model, IEnumerable<IFilter> filters)
        {
            var m = typeof(QueryBuilder).GetMethod("BuildQuery", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(IEnumerable<IFilter>) }, null).MakeGenericMethod(model);
            return (IQueryable)m.Invoke(null, new[] { filters });
        }

        /// <summary>
        ///     Construye un Query a partir de los filtros especificados.
        /// </summary>
        /// <typeparam name="T">Modelo para el cual construir un Query.</typeparam>
        /// <param name="filters">
        ///     Secuencia de filtros a aplicar al Query.
        ///     </param>
        /// <returns>
        ///     Un Query con todos los filtros especificados aplicados.
        /// </returns>
        public static IQueryable<T> BuildQuery<T>(IEnumerable<IFilter> filters) where T : ModelBase, new()
        {
            var q = Proteus.Infer(typeof(T)).All<T>();
            foreach (var j in filters)
            {
                q = q.Where((Expression<Func<T, bool>>)j.GetFilter(typeof(T)));
            }
            return q;
        }
    }
}