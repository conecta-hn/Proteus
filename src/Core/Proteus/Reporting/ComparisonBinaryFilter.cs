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

        public override Expression GetFilterOnly(Type model, ref ParameterExpression? entExp)
        {
            entExp ??= Expression.Parameter(model);

            return Comparer(
                    ToLower(
                        ToStringExp(
                            GetFromEntity(entExp),
                            Property.PropertyType)),
                    Expression.Constant(Value.ToLower()));
        }
    }
}