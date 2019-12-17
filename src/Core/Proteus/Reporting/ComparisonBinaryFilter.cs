/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Linq.Expressions;

namespace TheXDS.Proteus.Reporting
{
    /// <summary>
    ///     Clase abstracta que describe un <see cref="BinaryFilterBase"/> que
    ///     utiliza operandos de comparación.
    /// </summary>
    public abstract class ComparisonBinaryFilter : BinaryFilterBase
    {
        /// <summary>
        ///     Obtiene la expresión de comparación binaria.
        /// </summary>
        /// <param name="a">Primer operando de la expresión.</param>
        /// <param name="b">Segundo operando de la expresión.</param>
        /// <returns>
        ///     Una expresión que contiene la comparación entre ambos
        ///     operandos.
        /// </returns>
        protected abstract BinaryExpression Comparer(Expression a, Expression b);

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
                Comparer(
                    ToLower(
                        ToStringExp(
                            GetFromEntity(entExp),
                            Property.PropertyType)),
                    Expression.Constant(Value.ToLower())), entExp);
        }
    }





}