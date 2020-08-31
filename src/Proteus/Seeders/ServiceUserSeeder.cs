/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Threading.Tasks;
using TheXDS.MCART.Component;
using TheXDS.MCART.Security.Password;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.Models;

namespace TheXDS.Proteus.Seeders
{
    /// <summary>
    /// Semillador que genera usuarios administrativos predefinidos.
    /// </summary>
    public class ServiceUserSeeder : IAsyncDbSeeder
    {
        public string GetName => GetType().NameOf();

        public string InformationalVersion
        {
            get
            {
                return new AssemblyInfo(GetType().Assembly).InformationalVersion.OrNull() ?? GetType().Assembly.GetName().Version?.ToString().OrNull() ?? "1.0.0.0";
            }
        }

        /// <summary>
        /// Ejecuta la acción de semillado de la base de datos de forma
        /// asíncrona.
        /// </summary>
        /// <param name="service">
        /// Servicio sobre el cual ejecutar las acciones de semillado.
        /// </param>
        /// <param name="reporter">
        /// Objeto que permite reportar el progreso de la operación.
        /// </param>
        /// <returns>
        /// El resultado detallado de la operación de semillado de la base de
        /// datos.
        /// </returns>
        public async Task<DetailedResult> SeedAsync(IFullService service, IStatusReporter? reporter)
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
                    Enabled = true,
                    Interactive = true,
                    ScheduledPasswordChange = true,
                    PasswordHash = PasswordStorage.CreateHash("root".ToSecureString()),
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


        /// <summary>
        /// Realiza comprobaciones sobre la base de datos para determinar si
        /// debe ejecutarse o no este semillador.
        /// </summary>
        /// <param name="service">
        /// Servicio sobre el cual ejecutar las acciones de semillado.
        /// </param>
        /// <param name="reporter">
        /// Objeto que permite reportar el progreso de la operación.
        /// </param>
        /// <returns>
        /// <see langword="true"/> si debe ejecutarse el semillado en la base
        /// de datos, <see langword="false"/> en caso contrario.
        /// </returns>
        public async Task<bool> ShouldRunAsync(IReadAsyncService service, IStatusReporter? reporter)
        {
            reporter?.UpdateStatus("Comprobando usuarios de servicio...");
            return !await service.AnyAsync<User>(p => !p.Interactive);
        }
    }
}