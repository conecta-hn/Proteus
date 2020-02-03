/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models.Base;
using System.Collections.Generic;
using TheXDS.Proteus.Reporting;
using System;

namespace TheXDS.Proteus.Component
{
    /// <summary>
    /// Implementa un <see cref="ModelSearchFilter{T}"/> que puede filtrar
    /// entidades que implementen la interfaz <see cref="ISoftDeletable"/>.
    /// </summary>
    public class DeletablesFilter : ModelSearchFilter<ISoftDeletable>
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
        public override void AddFilter(ICollection<IFilter> mainFilter, ICollection<BinaryFilterBase> orFilter, string query)
        {
            mainFilter.Add(new EqualsFilter()
            {
                Property = typeof(ISoftDeletable).GetProperty("IsDeleted")!,
                Value = false.ToString()
            });
        }
    }
}