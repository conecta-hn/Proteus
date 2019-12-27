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
    ///     Implementa un <see cref="ModelSearchFilter{T}"/> que puede filtrar
    ///     modelos con la clase base o interfaz especificada, utilizando un
    ///     filtro de comparación <see cref="ContainsFilter"/>.
    /// </summary>
    public abstract class SimpleModelSearchFilter<T> : ModelSearchFilter<T>
    {
        private readonly string _propertyName;

        /// <summary>
        ///     Configura los filtros para permitir búsquedas que incluyan
        ///     entidades para las cuales esta instancia puede crearlos.
        /// </summary>
        /// <param name="mainFilter">
        ///     Filtro principal de la consulta. Se traduce en concatenaciones
        ///     de expresiones 
        ///     <see cref="System.Linq.Enumerable.Where{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>.
        /// </param>
        /// <param name="orFilter">
        ///     Filtro inclusivo de la consulta. Se traduce en una expresión
        ///     concatenada de comparaciones "OR".
        /// </param>
        /// <param name="query">
        ///     Cadena que describe la búsqueda solicitada por el usuario.
        /// </param>
        public override void AddFilter(ICollection<IFilter> mainFilter, ICollection<BinaryFilterBase> orFilter, string query)
        {
            orFilter.Add(new ContainsFilter()
            {
                Property = typeof(T).GetProperty(_propertyName)!,
                Value = query
            });
        }
        
        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="SimpleModelSearchFilter{T}"/>.
        /// </summary>
        /// <param name="propertyName">Nombre de la propiedad por la cual filtrar.</param>
        protected SimpleModelSearchFilter(string propertyName)
        {
            _propertyName = propertyName;
        }
    }
}