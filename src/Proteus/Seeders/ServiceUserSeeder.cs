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
    public class ServiceUserSeeder : IAsyncDbSeeder
    {
        public async Task<DetailedResult> SeedAsync(IFullService service, IStatusReporter reporter)
        {
            reporter?.UpdateStatus("Creando usuarios de servicio...");

            var role = await service.FirstOrDefaultAsync<UserRole>(p => p.DefaultGranted == SecurityFlags.Root);

            User Rootified(User u)
            {
                if (!(role is null))
                {
                    u.Roles.Add(role);
                }
                else
                {
                    u.DefaultGranted = SecurityFlags.Root;
                    u.ButtonBehavior = Models.Base.SecurityBehavior.Unlocked;
                    u.ModuleBehavior = Models.Base.SecurityBehavior.Unlocked;
                }
                return u;
            }

            return await service.AddAsync(new[] {
                Rootified(new User()
                {
                    Id = "root",
                    AllowMultiLogin = true,
                    Enabled = false,
                    Interactive = true,
                    Name = "Superusuario"
                }),
                Rootified(new User()
                {
                    Id = "apiwebservice",
                    AllowMultiLogin = true,
                    Enabled = true,
                    Interactive = false,
                    Name = "Servicio de API Web"
                }),
            });
        }


        public async Task<bool> ShouldRunAsync(IReadAsyncService service, IStatusReporter reporter)
        {
            reporter?.UpdateStatus("Comprobando usuarios de servicio...");
            return !await service.AnyAsync<User>(p => !p.Interactive);
        }
    }
}