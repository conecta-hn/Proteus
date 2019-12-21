/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Linq.Expressions;

namespace TheXDS.Proteus.Reporting
{
    /// <summary>
    ///     Describe un filtro de comparación que comprueba si una cadena
    ///     contiene a otra.
    /// </summary>
    public class ContainsFilter : BinaryFilterBase
    {
        /// <summary>
        ///     Obtiene una descripción para este <see cref="IFilter"/>.
        /// </summary>
        public override string Description => "contiene";

        /// <summary>
        ///     Obtiene la expresión a utilizar para filtrar una colección.
        /// </summary>
        /// <param name="model">
        ///     Modelo para la cual se generará la expresión.
        /// </param>
        /// <returns>
        ///     Una expresión que puede utilizarse para filtrar una <see cref="System.Linq.IQueryable{T}"/>
        /// </returns>
        public override sealed LambdaExpression GetFilter(Type model)
        {
            var entExp = Expression.Parameter(model);
            return Expression.Lambda(ToFunc(model),
                Expression.Call(
                    ToLower(
                        ToStringExp(
                            GetFromEntity(entExp),
                            Property.PropertyType)),
                    typeof(string).GetMethod("Contains", new Type[] { typeof(string) }),
                    GetValue()),
                entExp);
        }
    }
}