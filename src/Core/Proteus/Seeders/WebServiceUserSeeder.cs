/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Threading.Tasks;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.Seeders
{
    [SeederFor(typeof(UserService))]
    public class WebServiceUserSeeder : IAsyncDbSeeder
    {
        public async Task<DetailedResult> SeedAsync(IFullService service, IStatusReporter reporter)
        {
            reporter?.UpdateStatus("Creando tokens para usuarios de servicio...");
            var us = service as UserService;
            var r = await us.GenerateToken("apiwebservice", null, null, out var token);
            reporter?.UpdateStatus(100,$"Token para 'apiwebservice':\n{token}");
            return r;
        }

        public async Task<bool> ShouldRunAsync(IReadAsyncService service, IStatusReporter reporter)
        {
            reporter?.UpdateStatus("Comprobando tokens de servicios...");
            return !await service.AnyAsync<LoginToken>(p => p.Login.Id == "apiwebservice");
        }
    }
}