/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Linq.Expressions;

namespace TheXDS.Proteus.Reporting
{
    public class EqualsFilter : ComparisonBinaryFilter
    {
        public override string Description => "es igual a";

        protected override BinaryExpression Comparer(Expression a, Expression b)
        {
            return Expression.Equal(a, b);
        }
    }
    public class LessThanFilter : ComparisonBinaryFilter
    {
        public override string Description => "es menor que";
        protected override BinaryExpression Comparer(Expression a, Expression b)
        {
            return Expression.LessThan(a, b);
        }
    }
    public class NotEqualFilter : ComparisonBinaryFilter
    {
        public override string Description => "no es igual a";
        protected override BinaryExpression Comparer(Expression a, Expression b)
        {
            return Expression.NotEqual(a, b);
        }
    }
    public class LessThanOrEqualFilter : ComparisonBinaryFilter
    {
        public override string Description => "es menor o igual que";
        protected override BinaryExpression Comparer(Expression a, Expression b)
        {
            return Expression.LessThanOrEqual(a, b);
        }
    }
    public class GreaterThanFilter : ComparisonBinaryFilter
    {
        public override string Description => "es mayor que";
        protected override BinaryExpression Comparer(Expression a, Expression b)
        {
            return Expression.GreaterThan(a, b);
        }
    }
    public class GreaterThanOrEqualFilter : ComparisonBinaryFilter
    {
        public override string Description => "es mayor o igual a";
        protected override BinaryExpression Comparer(Expression a, Expression b)
        {
            return Expression.GreaterThanOrEqual(a, b);
        }
    }
}