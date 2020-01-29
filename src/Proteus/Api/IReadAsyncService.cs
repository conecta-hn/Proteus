/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TheXDS.Proteus.Models.Base;
using TheXDS.MCART.Attributes;

namespace TheXDS.Proteus.Api
{
    public interface IReadAsyncService : IReadServiceBase
    {
        [MethodKind(SecurityFlags.Search)]
        Task<bool> AnyAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : ModelBase, new();

        [MethodKind(SecurityFlags.New)]
        Task<TEntity> FirstAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : ModelBase, new();

        Task<TEntity?> FirstOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : ModelBase, new();

        Task<TEntity?> GetAsync<TEntity, TKey>(TKey id)
            where TEntity : ModelBase<TKey>, new()
            where TKey : IComparable<TKey>;

        [MethodKind(SecurityFlags.Search), Thunk]
        Task<bool> AnyAsync<TEntity>() where TEntity : ModelBase, new();

        [MethodKind(SecurityFlags.Search), Thunk]
        Task<bool> AllAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : ModelBase, new();

        Task<bool> ExistsAsync(ModelBase entity);

        Task<bool> ExistsAsync<TEntity>(string id) where TEntity : ModelBase, new();

        Task<bool> ExistsAsync(Type model, string id);

        [MethodKind(SecurityFlags.Search)]
        Task<TEntity?> GetAsync<TEntity>(string id) where TEntity : ModelBase, new();

        [MethodKind(SecurityFlags.Search)]
        Task<ModelBase?> GetAsync(Type model, string id);
    }
}