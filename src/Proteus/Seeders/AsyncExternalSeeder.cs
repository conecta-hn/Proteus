using System.Collections.Generic;
using System.Threading.Tasks;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.Models.Base;
using TheXDS.Proteus.Plugins;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Proteus.Seeders
{
    public abstract class AsyncExternalSeeder<T> : ExternalSeeder, IAsyncDbSeeder where T : ModelBase, new()
    {
        protected abstract IEnumerable<T> GenerateEntities();
        private string GetName => Name.OrNull() ?? GetType().NameOf() ?? GetType().Name;
        public override sealed Task<DetailedResult> SeedAsync(IFullService service, IStatusReporter reporter)
        {
            reporter?.UpdateStatus($"Creando {GetName}...");
            return service.AddAsync(GenerateEntities());
        }
        public override sealed async Task<bool> ShouldRunAsync(IReadAsyncService service, IStatusReporter reporter)
        {
            var r = false;
            reporter?.UpdateStatus($"Comprobando {GetName}...");
            do
            {
                try
                {
                    r = !await service.AnyAsync<T>().Throwable();
                    break;
                }
                catch
                {
                    if (_tries++ > 3)
                    {
                        Proteus.MessageTarget?.Stop($"Hubo un problema al comprobar {GetName} y se ha superado la cantidad permitida de reintentos. Se debe ejecutar nuevamente el semillado manualmente más tarde.");
                        break;
                    }
                    Proteus.MessageTarget?.Warning($"Hubo un problema al comprobar {GetName}. Reintentando por {_tries}a vez en 3000 ms...");
                    await Task.Delay(3000);
                }
            } while (true);
            return r;
        }
        private byte _tries = 0;
    }
}