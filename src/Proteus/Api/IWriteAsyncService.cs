/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Collections.Generic;
using System.Threading.Tasks;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Api
{
    public interface IWriteAsyncService
    {
        Task<DetailedResult> AddAsync<TEntity>(TEntity newEntity) where TEntity : ModelBase;
        Task<DetailedResult> AddAsync<TEntity>(IEnumerable<TEntity> newEntities) where TEntity : ModelBase;
        Task<DetailedResult> DeleteAsync<TEntity>(TEntity entity) where TEntity : ModelBase;
        Task<DetailedResult> DeleteAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : ModelBase;
        Task<DetailedResult> PurgeAsync<TEntity>(TEntity entity) where TEntity : ModelBase;
        Task<DetailedResult> PurgeAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : ModelBase;
        Task<DetailedResult> SaveAsync();
    }
}