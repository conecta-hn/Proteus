/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Linq;
using System.Linq.Expressions;

namespace TheXDS.Proteus.Reporting
{
    /// <summary>
    /// Define una serie de miembros a implementar por un tipo que permita obtener una expresión para filtrar un <see cref="IQueryable{T}"/>.
    /// </summary>
    public interface IFilter
    {
        /// <summary>
        /// Obtiene la expresión a utilizar para filtrar una colección.
        /// </summary>
        /// <param name="model">
        /// Modelo para la cual se generará la expresión.
        /// </param>
        /// <returns>
        /// Una expresión que puede utilizarse para filtrar una <see cref="IQueryable{T}"/>
        /// </returns>
        LambdaExpression GetFilter(Type model);
    }
}