/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public class OrFilter : IFilter, ICollection<BinaryFilterBase>
    {
        private List<BinaryFilterBase> _filters = new List<BinaryFilterBase>();

        public int Count => ((ICollection<BinaryFilterBase>)_filters).Count;

        public bool IsReadOnly => ((ICollection<BinaryFilterBase>)_filters).IsReadOnly;

        public void Add(BinaryFilterBase item)
        {
            ((ICollection<BinaryFilterBase>)_filters).Add(item);
        }

        public void Clear()
        {
            ((ICollection<ComparisonBinaryFilter>)_filters).Clear();
        }

        public bool Contains(BinaryFilterBase item)
        {
            return ((ICollection<ComparisonBinaryFilter>)_filters).Contains(item);
        }

        public void CopyTo(BinaryFilterBase[] array, int arrayIndex)
        {
            ((ICollection<BinaryFilterBase>)_filters).CopyTo(array, arrayIndex);
        }

        public IEnumerator<BinaryFilterBase> GetEnumerator()
        {
            return ((ICollection<ComparisonBinaryFilter>)_filters).GetEnumerator();
        }

        public LambdaExpression GetFilter(Type model)
        {
            ParameterExpression? ent = null;
            var exp = _filters.First().GetFilterOnly(model, ref ent);
            
            foreach (var j in _filters.Skip(1))
            {
                exp = Expression.Or(exp, j.GetFilterOnly(model, ref ent));
            }

            return Expression.Lambda(typeof(Func<,>).MakeGenericType(model, typeof(bool)), exp, ent);
                
        }

        public bool Remove(BinaryFilterBase item)
        {
            return ((ICollection<BinaryFilterBase>)_filters).Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((ICollection<ComparisonBinaryFilter>)_filters).GetEnumerator();
        }
    }
}