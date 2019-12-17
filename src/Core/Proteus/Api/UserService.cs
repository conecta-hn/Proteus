/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Linq;
using System.Threading.Tasks;
using TheXDS.Proteus.Context;
using TheXDS.Proteus.Models;
using TheXDS.Proteus.Seeders;
using TheXDS.MCART.Attributes;
using static TheXDS.Proteus.Proteus;
using System.Security;
using TheXDS.MCART.Security.Password;
using TheXDS.MCART.Types.Extensions;
using System;
using System.Security.Cryptography;

namespace TheXDS.Proteus.Api
{
    [Seeder(typeof(ServiceUserSeeder))]
    [Name("Gestión de sistema"), Priority(-1)]
    public class UserService : Service<UserContext>, ILoginSource
    {
        private static UserService Svc => Proteus.Service<UserService>();
        public UserService()
        {
            RegisterLoginSource(this);
        }

        /// <summary>
        ///     Comprueba las credenciales brindadas para realizar un inicio de
        ///     sesión.
        /// </summary>
        /// <param name="user">Usuario que inicia sesión.</param>
        /// <param name="password">Contraseña.</param>
        /// <param name="checkProteusMode">
        ///     Si se establece en <see langword="true"/>, se comprobará el
        ///     modo de interactividad de Proteus contra la configuración de
        ///     la misma sobre el usuario que inicia sesión.
        /// </param>
        /// <returns>
        ///     Un objeto <see cref="LoginResult"/> que describe el resultado
        ///     de la operación.
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
            
            switch (await PasswordStorage.VerifyPasswordAsync(password, u.PasswordHash))
            {
                case null: return false;
                case false: return LoginResultCode.InvalidPassword;
                default:
                    return new LoginResult(u);
            }
        }

        /// <summary>
        ///     Comprueba las credenciales brindadas para realizar un inicio de
        ///     sesión.
        /// </summary>
        /// <param name="user">Usuario que inicia sesión.</param>
        /// <param name="password">Contraseña.</param>
        /// <returns>
        ///     Un objeto <see cref="LoginResult"/> que describe el resultado
        ///     de la operación.
        /// </returns>
        Task<LoginResult> ILoginSource.Login(string user, SecureString password) => Login(user, password, false);

        /// <summary>
        ///     Comprueba las credenciales brindadas para realizar un inicio de
        ///     sesión.
        /// </summary>
        /// <param name="user">Usuario que inicia sesión.</param>
        /// <param name="password">Contraseña.</param>
        /// <returns>
        ///     Un objeto <see cref="LoginResult"/> que describe el resultado
        ///     de la operación.
        /// </returns>
        public Task<LoginResult> Login(string user, SecureString password) => Login(user, password, true);

        [MethodKind(SecurityFlags.Admin)]
        public Task<DetailedResult> GenerateToken(User user, string host, DateTime? @void, bool onceOnly, out string token)
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
        public Task<DetailedResult> GenerateToken(string user, string host, DateTime? @void, out string token)
        {
            return GenerateToken(Get<User>(user), host, @void, false, out token);
        }
        public Task<DetailedResult> GenerateToken(string user, string host, bool @void, out string token)
        {
            return GenerateToken(user, host, @void ? DateTime.Today + TimeSpan.FromDays(365) : (DateTime?)null, out token);
        }
        public Task<DetailedResult> GenerateToken(string user, string host, int days, out string token)
        {
            return GenerateToken(user, host, DateTime.Today + TimeSpan.FromDays(days), out token);
        }
        public Task<DetailedResult> GenerateToken(string user, string host, out string token)
        {
            return GenerateToken(user, host, true, out token);
        }
        public Task<DetailedResult> GenerateToken(string user, out string token)
        {
            return GenerateToken(user, Environment.MachineName, true, out token);
        }
        public Task<DetailedResult> GenerateToken(out string token)
        {
            return GenerateToken(Session.Id, out token);
        }
        public Task<DetailedResult> GenerateToken(bool onceOnly, out string token)
        {
            return GenerateToken(Session.Id, onceOnly, out token);
        }
        public Task<DetailedResult> GenerateToken(string user, bool onceOnly, out string token)
        {
            return GenerateToken(user, onceOnly, Environment.MachineName, out token);
        }
        public Task<DetailedResult> GenerateToken(string user, bool onceOnly, string host, out string token)
        {
            return GenerateToken(user, onceOnly, Environment.MachineName, true, out token);
        }
        public Task<DetailedResult> GenerateToken(string user, bool onceOnly, string host, bool @void, out string token)
        {
            return GenerateToken(user,onceOnly, host, @void ? DateTime.Today + TimeSpan.FromDays(365) : (DateTime?)null, out token);
        }
        public Task<DetailedResult> GenerateToken(string user,bool onceOnly, string host, DateTime? @void, out string token)
        {
            return GenerateToken(Get<User>(user), host, @void, onceOnly, out token);
        }
        [MethodKind(SecurityFlags.None)]
        public Task<DetailedResult> ConsumeToken(LoginToken token)
        {
            if (token.OnceOnly) Context.LoginTokens.Remove(token);
            return InternalSaveAsync();
        }

        public static IQueryable<Aviso> AllAvisos => Svc.All<Aviso>();
        public static IQueryable<User> InteractiveUsers => Svc.All<User>().Where(p => p.Interactive);

        public static async Task PurgeAllTokensAsync()
        {
            await Svc.PurgeAsync(await Svc.AllAsync<LoginToken>());

        }
    }
}