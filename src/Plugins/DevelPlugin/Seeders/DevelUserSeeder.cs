/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Threading.Tasks;
using TheXDS.MCART.Attributes;
using TheXDS.MCART.Security.Password;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Plugins;

[assembly: Name("Development Seeders")]
namespace TheXDS.Proteus
{
    [Name("Seeder para usuarios de desarrollo")]
    [SeederFor(typeof(UserService))]
    public class DevelUserSeeder : ExternalSeeder
    {
        public override Task<DetailedResult> SeedAsync(IFullService service, IStatusReporter reporter)
        {
            return service.AddAsync(new User
            {
                AllowMultiLogin = true,
                ButtonBehavior = Models.Base.SecurityBehavior.Unlocked,
                DefaultGranted = SecurityFlags.Root,
                DefaultRevoked = SecurityFlags.None,
                Enabled = true,
                Id = "devel",
                Interactive = true,
                IsDeleted = false,
                ModuleBehavior = Models.Base.SecurityBehavior.Unlocked,
                Name = "Superusuario de desarrollo",
                PasswordHash = PasswordStorage.CreateHash("devel".ToSecureString()),
                ScheduledPasswordChange = true
            });                        
        }

        public override async Task<bool> ShouldRunAsync(IReadAsyncService service, IStatusReporter reporter)
        {
            return (await service.GetAsync<User, string>("devel")) is null;
        }
    }
}