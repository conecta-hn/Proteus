/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Models.Base;
using System;
using System.Linq.Expressions;

namespace TheXDS.Proteus.Crud.Base
{
    /// <summary>
    /// Describe una serie de miembros a implementar por un tipo que
    /// permita describir cualquier tipo de listas.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TItem"></typeparam>
    public interface IListBasePropertyDescriptor<out T, TItem> where T : IDataPropertyDescriptor where TItem : ModelBase
    {
        /// <summary>
        /// Agrega columnas personalizadas al control de lista asociado a 
        /// este descriptor.
        /// </summary>
        /// <param name="header">Encabezado de columna.</param>
        /// <param name="path">Ruta de propiedad a asociar.</param>
        /// <returns>
        /// Este mismo descriptor como un <typeparamref name="T"/> para
        /// utilizar sintáxis Fluent.
        /// </returns>
        T Column(string header, string path);

        /// <summary>
        /// Agrega columnas personalizadas al control de lista asociado a 
        /// este descriptor.
        /// </summary>
        /// <param name="header">Encabezado de columna.</param>
        /// <param name="property">Selector de propiedad a asociar.</param>
        /// <returns>
        /// Este mismo descriptor como un <typeparamref name="T"/> para
        /// utilizar sintáxis Fluent.
        /// </returns>
        T Column(string header, Expression<Func<TItem, object>> property);
    }
}