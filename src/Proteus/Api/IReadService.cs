/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Api
{
    /// <summary>
    /// Define una serie de miembros a implementar por un tipo que exponga
    /// funcionalidad de lectura para un servicio.
    /// </summary>
    public interface IReadService : IReadServiceBase
    {
        /// <summary>
        /// Obtiene una enumeración de todas las entidades del modelo
        /// especificado.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Modelo a obtener.
        /// </typeparam>
        /// <returns>
        /// Una enumeración de todas las entidades del modelo especificado.
        /// </returns>
        [MethodKind(SecurityFlags.Read)]
        IQueryable<TEntity> All<TEntity>() where TEntity : ModelBase, new();

        /// <summary>
        /// Obtiene una enumeración de todas las entidades del modelo base
        /// especificado.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Modelo a obtener.
        /// </typeparam>
        /// <returns>
        /// Una enumeración de todas las entidades del modelo base
        /// especificado.
        /// </returns>
        [MethodKind(SecurityFlags.Read)]
        IEnumerable<TEntity> AllBase<TEntity>() where TEntity : ModelBase;

        /// <summary>
        /// Obtiene un valor que indica si la entidad especificada existe en la
        /// base de datos.
        /// </summary>
        /// <param name="entity">Entidad a comprobar.</param>
        /// <returns>
        /// <see langword="true"/> si la entidad existe en la base de datos,
        /// <see langword="false"/> en caso contrario.
        /// </returns>
        bool Exists(ModelBase entity);

        /// <summary>
        /// Obtiene un valor que indica si existe una entidad con el Id 
        /// especificado en la base de datos.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Modelo a buscar.
        /// </typeparam>
        /// <param name="id">Id de la entidad a buscar.</param>
        /// <returns>
        /// <see langword="true"/> si la entidad existe en la base de datos,
        /// <see langword="false"/> en caso contrario.
        /// </returns>
        bool Exists<TEntity>(string id) where TEntity : ModelBase, new();

        /// <summary>
        /// Obtiene un valor que indica si existe una entidad con el Id 
        /// especificado en la base de datos.
        /// </summary>
        /// <param name="model">
        /// Modelo a buscar.
        /// </param>
        /// <param name="id">Id de la entidad a buscar.</param>
        /// <returns>
        /// <see langword="true"/> si la entidad existe en la base de datos,
        /// <see langword="false"/> en caso contrario.
        /// </returns>
        bool Exists(Type model, string id);

        /// <summary>
        /// Obtiene a la primera entidad que cumpla con la condición
        /// especificada.
        /// </summary>
        /// <typeparam name="TEntity">Modelo a obtener.</typeparam>
        /// <param name="predicate">
        /// Función que determina si la entidad a devolver cumple con la 
        /// condición.
        /// </param>
        /// <returns>
        /// La primera entidad que coincida con la condición especificada.
        /// </returns>
        [MethodKind(SecurityFlags.Search)]
        TEntity First<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : ModelBase, new();

        /// <summary>
        /// Obtiene a la primera entidad que cumpla con la condición
        /// especificada.
        /// </summary>
        /// <typeparam name="TEntity">Modelo a obtener.</typeparam>
        /// <param name="predicate">
        /// Función que determina si la entidad a devolver cumple con la
        /// condición.
        /// </param>
        /// <returns>
        /// La primera entidad que coincida con la condición especificada, o 
        /// <see langword="null"/> si ninguna entidad cumple con la condición 
        /// especificada.
        /// </returns>
        [MethodKind(SecurityFlags.Search)]
        TEntity? FirstOrDefault<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : ModelBase, new();

        /// <summary>
        /// Obtiene una entidad con el Id especificado.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Modelo de la entidad a obtener.
        /// </typeparam>
        /// <typeparam name="TKey">
        /// Tipo de Id de la entidad a obtener.
        /// </typeparam>
        /// <param name="id">Id de la entidad a obtener.</param>
        /// <returns>
        /// Una entidad cuyo Id sea el especificado, o <see langword="null"/>
        /// si no existe ninguna entidad con el Id especificado.
        /// </returns>
        [MethodKind(SecurityFlags.Search)]
        TEntity? Get<TEntity, TKey>(TKey id)
            where TEntity : ModelBase<TKey>, new()
            where TKey : IComparable<TKey>, IEquatable<TKey>;

        /// <summary>
        /// Obtiene una entidad con el Id especificado.
        /// </summary>
        /// <typeparam name="TEntity">
        /// Modelo de la entidad a obtener.
        /// </typeparam>
        /// <param name="id">Id de la entidad a obtener.</param>
        /// <returns>
        /// Una entidad cuyo Id sea el especificado, o <see langword="null"/>
        /// si no existe ninguna entidad con el Id especificado.
        /// </returns>
        [MethodKind(SecurityFlags.Search)]
        TEntity? Get<TEntity>(string id) where TEntity : ModelBase, new();

        /// <summary>
        /// Obtiene una entidad con el Id especificado.
        /// </summary>
        /// <param name="model">
        /// Modelo de la entidad a obtener.
        /// </param>
        /// <param name="id">Id de la entidad a obtener.</param>
        /// <returns>
        /// Una entidad cuyo Id sea el especificado, o <see langword="null"/>
        /// si no existe ninguna entidad con el Id especificado.
        /// </returns>
        [MethodKind(SecurityFlags.Search)]
        ModelBase? Get(Type model, string id);
    }
}