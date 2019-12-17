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
    public interface IReadService : IReadServiceBase
    {
        [MethodKind(SecurityFlags.Read)]
        IQueryable<TEntity> All<TEntity>() where TEntity : ModelBase, new();
        [MethodKind(SecurityFlags.Read)]
        IEnumerable<TEntity> AllBase<TEntity>() where TEntity : ModelBase;
        bool Exists(ModelBase entity);
        bool Exists<TEntity>(string id) where TEntity : ModelBase, new();
        bool Exists(Type model, string id);
        [MethodKind(SecurityFlags.Search)]
        TEntity First<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : ModelBase, new();
        [MethodKind(SecurityFlags.Search)]
        TEntity FirstOrDefault<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : ModelBase, new();
        [MethodKind(SecurityFlags.Search)]
        TEntity Get<TEntity, TKey>(TKey id)
            where TEntity : ModelBase<TKey>, new()
            where TKey : IComparable<TKey>, IEquatable<TKey>;
        [MethodKind(SecurityFlags.Search)]
        TEntity Get<TEntity>(string id) where TEntity : ModelBase, new();
        [MethodKind(SecurityFlags.Search)]
        ModelBase Get(Type model, string id);
    }
}