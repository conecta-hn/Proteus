/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Threading.Tasks;
using TheXDS.MCART.Attributes;
using TheXDS.MCART.Security.Password;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Context;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Seeders;
using static TheXDS.Proteus.Proteus;

namespace TheXDS.Proteus.Api
{
    /// <summary>
    /// Servicio que gestion las sesiones dentro del sistema Proteus.
    /// </summary>
    [Seeder(typeof(ServiceUserSeeder))]
    [Name("Gestión de sistema"), Priority(-1)]
    public class UserService : Service<UserContext>, ILoginSource
    {
        private static UserService Svc => Proteus.Service<UserService>()!;

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="UserService"/>.
        /// </summary>
        public UserService()
        {
            RegisterLoginSource(this);
        }

        /// <summary>
        /// Comprueba las credenciales brindadas para realizar un inicio de
        /// sesión.
        /// </summary>
        /// <param name="user">Usuario que inicia sesión.</param>
        /// <param name="password">Contraseña.</param>
        /// <param name="checkProteusMode">
        /// Si se establece en <see langword="true"/>, se comprobará el
        /// modo de interactividad de Proteus contra la configuración de
        /// la misma sobre el usuario que inicia sesión.
        /// </param>
        /// <returns>
        /// Un objeto <see cref="LoginResult"/> que describe el resultado
        /// de la operación.
        /// </returns>
        public async Task<LoginResult> Login(string user, SecureString password, bool checkProteusMode)
        {
            if (user.IsEmpty()) return LoginResultCode.NoUser;
            var u = await GetAsync<User, string>(user);

            if (u is null) return LoginResultCode.UnknownUser;
            if (checkProteusMode && u.Interactive != Interactive)
                return Interactive ? LoginResultCode.NotInteractive : LoginResultCode.NotSvcUser;

            if (!u.Enabled) return LoginResultCode.DisabledUser;

            if (!(u.PasswordHash?.Any() ?? false)) return LoginResultCode.NoPassword;

            return (await PasswordStorage.VerifyPasswordAsync(password, u.PasswordHash)) switch
            {
                null => false,
                false => LoginResultCode.InvalidPassword,
                true => new LoginResult(u),
            };
        }

        /// <summary>
        /// Comprueba las credenciales brindadas para realizar un inicio de
        /// sesión.
        /// </summary>
        /// <param name="user">Usuario que inicia sesión.</param>
        /// <param name="password">Contraseña.</param>
        /// <returns>
        /// Un objeto <see cref="LoginResult"/> que describe el resultado
        /// de la operación.
        /// </returns>
        Task<LoginResult> ILoginSource.Login(string user, SecureString password) => Login(user, password, false);

        /// <summary>
        /// Comprueba las credenciales brindadas para realizar un inicio de
        /// sesión.
        /// </summary>
        /// <param name="user">Usuario que inicia sesión.</param>
        /// <param name="password">Contraseña.</param>
        /// <returns>
        /// Un objeto <see cref="LoginResult"/> que describe el resultado
        /// de la operación.
        /// </returns>
        public Task<LoginResult> Login(string user, SecureString password) => Login(user, password, true);

        /// <summary>
        /// Genera un token de seguriad para el usuario y host especificados.
        /// </summary>
        /// <param name="user">
        /// Usuario para el cual se generará el token.
        /// </param>
        /// <param name="host">
        /// Host en el cual el token será válido. <see langword="null"/> le
        /// dará validez en cualquier host.
        /// </param>
        /// <param name="void">Marca de tiempo de vencimiento.
        /// <see langword="null"/> hará que el token no puede vencerse.
        /// </param>
        /// <param name="onceOnly">
        /// Si se establece el <see langword="true"/>, el token creado solo
        /// podrá se utilizado una vez para iniciar sesión en el sistema.
        /// </param>
        /// <param name="token">
        /// Parámetro de salida. Cadena que contiene el token generado.
        /// </param>
        /// <returns>
        /// Un resultado detallado que describe el resultado de la operación.
        /// </returns>
        [MethodKind(SecurityFlags.Admin)]
        public Task<DetailedResult> GenerateToken(User user, string? host, DateTime? @void, bool onceOnly, out string? token)
        {
            if (!Elevate())
            {
                token = null;
                return Task.FromResult(new DetailedResult(Result.Forbidden));
            }

            var b = new byte[96];
            using (var rng = RandomNumberGenerator.Create()) rng.GetBytes(b);                        
            token = Convert.ToBase64String(b);
            Add(new LoginToken()
            {
                Id = token,
                ComputerName = host,
                Login = user,
                OnceOnly = onceOnly,
                Void = @void
            });
            AfterElevation();
            return SaveAsync();
        }

        /// <summary>
        /// Genera un token de seguriad para el usuario y host especificados.
        /// </summary>
        /// <param name="user">
        /// Usuario para el cual se generará el token.
        /// </param>
        /// <param name="host">
        /// Host en el cual el token será válido. <see langword="null"/> le
        /// dará validez en cualquier host.
        /// </param>
        /// <param name="void">Marca de tiempo de vencimiento.
        /// <see langword="null"/> hará que el token no puede vencerse.
        /// </param>
        /// <param name="token">
        /// Parámetro de salida. Cadena que contiene el token generado.
        /// </param>
        /// <returns>
        /// Un resultado detallado que describe el resultado de la operación.
        /// </returns>
        public Task<DetailedResult> GenerateToken(string user, string? host, DateTime? @void, out string? token)
        {
            var u = Get<User>(user);
            if (u is null)
            {
                token = null;
                return Task.FromResult(DetailedResult.Fail);
            }
            return GenerateToken(u, host, @void, false, out token);
        }

        /// <summary>
        /// Genera un token de seguriad para el usuario y host especificados.
        /// </summary>
        /// <param name="user">
        /// Usuario para el cual se generará el token.
        /// </param>
        /// <param name="host">
        /// Host en el cual el token será válido. <see langword="null"/> le
        /// dará validez en cualquier host.
        /// </param>
        /// <param name="void">Determina si el token deberá vencerse.
        /// <see langword="true"/> le da una vida de 365 días al token
        /// generado.
        /// </param>
        /// <param name="token">
        /// Parámetro de salida. Cadena que contiene el token generado.
        /// </param>
        /// <returns>
        /// Un resultado detallado que describe el resultado de la operación.
        /// </returns>
        public Task<DetailedResult> GenerateToken(string user, string? host, bool @void, out string? token)
        {
            return GenerateToken(user, host, @void ? DateTime.Today + TimeSpan.FromDays(365) : (DateTime?)null, out token);
        }

        /// <summary>
        /// Genera un token de seguriad para el usuario y host especificados.
        /// </summary>
        /// <param name="user">
        /// Usuario para el cual se generará el token.
        /// </param>
        /// <param name="host">
        /// Host en el cual el token será válido. <see langword="null"/> le
        /// dará validez en cualquier host.
        /// </param>
        /// <param name="days">
        /// Cantidad de días de vida del token generado.
        /// </param>
        /// <param name="token">
        /// Parámetro de salida. Cadena que contiene el token generado.
        /// </param>
        /// <returns>
        /// Un resultado detallado que describe el resultado de la operación.
        /// </returns>
        public Task<DetailedResult> GenerateToken(string user, string? host, int days, out string? token)
        {
            return GenerateToken(user, host, DateTime.Today + TimeSpan.FromDays(days), out token);
        }

        /// <summary>
        /// Genera un token de seguriad para el usuario y host especificados.
        /// </summary>
        /// <param name="user">
        /// Usuario para el cual se generará el token.
        /// </param>
        /// <param name="host">
        /// Host en el cual el token será válido. <see langword="null"/> le
        /// dará validez en cualquier host.
        /// </param>
        /// <param name="token">
        /// Parámetro de salida. Cadena que contiene el token generado.
        /// </param>
        /// <returns>
        /// Un resultado detallado que describe el resultado de la operación.
        /// </returns>
        public Task<DetailedResult> GenerateToken(string user, string? host, out string? token)
        {
            return GenerateToken(user, host, true, out token);
        }

        /// <summary>
        /// Genera un token de seguriad para el usuario y host especificados.
        /// </summary>
        /// <param name="user">
        /// Usuario para el cual se generará el token.
        /// </param>
        /// <param name="token">
        /// Parámetro de salida. Cadena que contiene el token generado.
        /// </param>
        /// <returns>
        /// Un resultado detallado que describe el resultado de la operación.
        /// </returns>
        public Task<DetailedResult> GenerateToken(string user, out string? token)
        {
            return GenerateToken(user, Environment.MachineName, true, out token);
        }

        /// <summary>
        /// Genera un token de seguriad para el usuario y host especificados.
        /// </summary>
        /// <param name="token">
        /// Parámetro de salida. Cadena que contiene el token generado.
        /// </param>
        /// <returns>
        /// Un resultado detallado que describe el resultado de la operación.
        /// </returns>
        public Task<DetailedResult> GenerateToken(out string? token)
        {
            return GenerateToken(Session?.Id.OrEmpty(), out token);
        }

        /// <summary>
        /// Genera un token de seguriad para el usuario y host especificados.
        /// </summary>
        /// <param name="onceOnly">
        /// Si se establece el <see langword="true"/>, el token creado solo
        /// podrá se utilizado una vez para iniciar sesión en el sistema.
        /// </param>
        /// <param name="token">
        /// Parámetro de salida. Cadena que contiene el token generado.
        /// </param>
        /// <returns>
        /// Un resultado detallado que describe el resultado de la operación.
        /// </returns>
        public Task<DetailedResult> GenerateToken(bool onceOnly, out string? token)
        {
            return GenerateToken(Session?.Id.OrEmpty(), onceOnly, out token);
        }

        /// <summary>
        /// Genera un token de seguriad para el usuario y host especificados.
        /// </summary>
        /// <param name="user">
        /// Usuario para el cual se generará el token.
        /// </param>
        /// <param name="onceOnly">
        /// Si se establece el <see langword="true"/>, el token creado solo
        /// podrá se utilizado una vez para iniciar sesión en el sistema.
        /// </param>
        /// <param name="token">
        /// Parámetro de salida. Cadena que contiene el token generado.
        /// </param>
        /// <returns>
        /// Un resultado detallado que describe el resultado de la operación.
        /// </returns>
        public Task<DetailedResult> GenerateToken(string user, bool onceOnly, out string? token)
        {
            return GenerateToken(user, onceOnly, Environment.MachineName, out token);
        }

        /// <summary>
        /// Genera un token de seguriad para el usuario y host especificados.
        /// </summary>
        /// <param name="user">
        /// Usuario para el cual se generará el token.
        /// </param>
        /// <param name="host">
        /// Host en el cual el token será válido. <see langword="null"/> le
        /// dará validez en cualquier host.
        /// </param>
        /// <param name="onceOnly">
        /// Si se establece el <see langword="true"/>, el token creado solo
        /// podrá se utilizado una vez para iniciar sesión en el sistema.
        /// </param>
        /// <param name="token">
        /// Parámetro de salida. Cadena que contiene el token generado.
        /// </param>
        /// <returns>
        /// Un resultado detallado que describe el resultado de la operación.
        /// </returns>
        public Task<DetailedResult> GenerateToken(string user, bool onceOnly, string host, out string? token)
        {
            return GenerateToken(user, onceOnly, host, true, out token);
        }

        /// <summary>
        /// Genera un token de seguriad para el usuario y host especificados.
        /// </summary>
        /// <param name="user">
        /// Usuario para el cual se generará el token.
        /// </param>
        /// <param name="host">
        /// Host en el cual el token será válido. <see langword="null"/> le
        /// dará validez en cualquier host.
        /// </param>
        /// <param name="void">Marca de tiempo de vencimiento.
        /// <see langword="null"/> hará que el token no puede vencerse.
        /// </param>
        /// <param name="onceOnly">
        /// Si se establece el <see langword="true"/>, el token creado solo
        /// podrá se utilizado una vez para iniciar sesión en el sistema.
        /// </param>
        /// <param name="token">
        /// Parámetro de salida. Cadena que contiene el token generado.
        /// </param>
        /// <returns>
        /// Un resultado detallado que describe el resultado de la operación.
        /// </returns>
        public Task<DetailedResult> GenerateToken(string user, bool onceOnly, string host, bool @void, out string? token)
        {
            return GenerateToken(user, onceOnly, host, @void ? DateTime.Today + TimeSpan.FromDays(365) : (DateTime?)null, out token);
        }

        /// <summary>
        /// Genera un token de seguriad para el usuario y host especificados.
        /// </summary>
        /// <param name="user">
        /// Usuario para el cual se generará el token.
        /// </param>
        /// <param name="host">
        /// Host en el cual el token será válido. <see langword="null"/> le
        /// dará validez en cualquier host.
        /// </param>
        /// <param name="void">Marca de tiempo de vencimiento.
        /// <see langword="null"/> hará que el token no puede vencerse.
        /// </param>
        /// <param name="onceOnly">
        /// Si se establece el <see langword="true"/>, el token creado solo
        /// podrá se utilizado una vez para iniciar sesión en el sistema.
        /// </param>
        /// <param name="token">
        /// Parámetro de salida. Cadena que contiene el token generado.
        /// </param>
        /// <returns>
        /// Un resultado detallado que describe el resultado de la operación.
        /// </returns>
        public Task<DetailedResult> GenerateToken(string user, bool onceOnly, string host, DateTime? @void, out string? token)
        {
            var u = Get<User>(user);
            if (u is null)
            {
                token = null;
                return Task.FromResult(DetailedResult.Fail);
            }
            return GenerateToken(u, host, @void, onceOnly, out token);
        }

        /// <summary>
        /// Consume un token de inicio de sesión.
        /// </summary>
        /// <param name="token">Token a consumir.</param>
        /// <returns>
        /// Un resultado detallado que describe el resultado de la operación.
        /// </returns>
        [MethodKind(SecurityFlags.None)]
        public Task<DetailedResult> ConsumeToken(LoginToken token)
        {
            if (token.OnceOnly) Context.LoginTokens.Remove(token);
            return InternalSaveAsync();
        }

        /// <summary>
        /// Obtiene un Query que incluye todos los avisos del sistema.
        /// </summary>
        public static IQueryable<Aviso> AllAvisos => Svc.All<Aviso>();

        /// <summary>
        /// Obtiene un Query que incluye a todos los usuarios interactivos del
        /// sistema.
        /// </summary>
        public static IQueryable<User> InteractiveUsers => Svc.All<User>().Where(p => p.Interactive);

        /// <summary>
        /// Purga todos los tokens de seguridad de la base de datos.
        /// </summary>
        /// <returns>
        /// Un resultado detallado que describe el resultado de la operación.
        /// </returns>
        public static async Task PurgeAllTokensAsync()
        {
            await Svc.PurgeAsync(await Svc.AllAsync<LoginToken>());
        }
    }
}