using System.Collections.Generic;
using System.Threading.Tasks;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.Models.Base;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Proteus.Seeders
{
    public abstract class AsyncDbSeeder<T> : IAsyncDbSeeder where T: ModelBase, new()
    {
        protected virtual string Name { get; }
        protected abstract IEnumerable<T> GenerateEntities();
        private string GetName => Name.OrNull() ?? typeof(T).NameOf() ?? GetType().NameOf() ?? GetType().Name;
        public Task<DetailedResult> SeedAsync(IFullService service, IStatusReporter reporter)
        {
            reporter?.UpdateStatus($"Creando {GetName}...");
            return service.AddAsync(GenerateEntities());
        }
        public virtual async Task<bool> ShouldRunAsync(IReadAsyncService service, IStatusReporter reporter)
        {
            reporter?.UpdateStatus($"Comprobando {GetName}...");
            return !await service.AnyAsync<T>();
        }
    }
}