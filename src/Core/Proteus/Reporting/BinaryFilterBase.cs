/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Linq.Expressions;
using System.Reflection;
using TheXDS.MCART.Types;

namespace TheXDS.Proteus.Reporting
{
    /// <summary>
    ///     Clase base para todos los filtros de expresión binaria.
    /// </summary>
    public abstract class BinaryFilterBase : IFilter, IDescriptible, IPropComparisonFilter
    {
        /// <summary>
        ///     Crea el tipo de delegado necesario para filtrar una colección
        ///     por medio del método de extensión
        ///     <see cref="System.Linq.Queryable.Where{TSource}(System.Linq.IQueryable{TSource}, Expression{Func{TSource, bool}})"/>
        /// </summary>
        /// <param name="model">
        ///     Modelo a utilizar como parámetro genérico del delegado.
        /// </param>
        /// <returns>
        ///     Un tipo que describe a un delegado 
        ///     <see cref="Func{T, TResult}"/>.
        /// </returns>
        protected static Type ToFunc(Type model)
        {
            return typeof(Func<,>).MakeGenericType(model, typeof(bool));
        }

        /// <summary>
        ///     Obtiene el valor de la propiedad desde la entidad.
        /// </summary>
        /// <param name="source">
        ///     Expresión desde la cual obtener la entidad.
        /// </param>
        /// <returns>
        ///     Una expresión que obtiene el valor de una propiedad desde la
        ///     entidad.
        /// </returns>
        protected Expression GetFromEntity(Expression source)
        {
            return Expression.Property(source, Property);
        }

        /// <summary>
        ///     Convierte una expresión en una cadena.
        /// </summary>
        /// <param name="expression">
        ///     Expresión desde la cual obtener el valor.
        /// </param>
        /// <param name="valType">Tipo de valor.</param>
        /// <returns>
        ///     Una expresión que convierte en una cadena el valor obtenido
        ///     desde la expresión especificada.
        /// </returns>
        protected static Expression ToStringExp(Expression expression, Type valType)
        {
            return Expression.Call(expression, valType.GetMethod("ToString", Type.EmptyTypes));
        }

        /// <summary>
        ///     Convierte una cadena a minúsculas.
        /// </summary>
        /// <param name="expression">
        ///     Expresión desde la cual obtener la cadena.
        /// </param>
        /// <returns>
        ///     Una expresión que convierte la cadena obtenida desde la
        ///     expresión especificada a minúsculas.
        /// </returns>
        protected static Expression ToLower(Expression expression)
        {
            return Expression.Call(expression, typeof(string).GetMethod("ToLower", Type.EmptyTypes));
        }

        /// <summary>
        ///     Obtiene una referencia al valor constante relacionado a esta instancia.
        /// </summary>
        /// <returns></returns>
        protected Expression GetValue()
        {
            return Expression.Constant(Value.ToLower());
        }

        /// <summary>
        ///     Obtiene o establece la propiedad del modelo para la cual se
        ///     crearán las expresiones.
        /// </summary>
        public PropertyInfo Property { get; set; }

        /// <summary>
        ///     Obtiene o establece el valor constante que se utilizará para
        ///     comparar contra el valor de la propiedad.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        ///     Obtiene una descripción para este <see cref="IFilter"/>.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        ///     Obtiene la expresión a utilizar para filtrar una colección.
        /// </summary>
        /// <param name="model">
        ///     Modelo para la cual se generará la expresión.
        /// </param>
        /// <returns>
        ///     Una expresión que puede utilizarse para filtrar una <see cref="System.Linq.IQueryable{T}"/>
        /// </returns>
        public virtual LambdaExpression GetFilter(Type model)
        {
            ParameterExpression? entExp = null;
            return Expression.Lambda(ToFunc(model), GetFilterOnly(model, ref entExp), entExp);
        }

        public abstract Expression GetFilterOnly(Type model, ref ParameterExpression? entExp);
    }
}