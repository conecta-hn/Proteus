/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

#define LoginWithJsonData

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using TheXDS.MCART;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Controllers.Base;

namespace TheXDS.Proteus.Controllers
{
    /// <summary>
    /// Describe una sesión de usuario activa en el servicio web.
    /// </summary>
#pragma warning disable CA1815 // Override equals and operator equals on value types
    public struct ActiveSession
    {
        /// <summary>
        /// Origen de inicio de sesión para la instancia actual.
        /// </summary>
        public ILoginSource Source { get; }
        
        /// <summary>
        /// Token de seguridad generado para esta sesión.
        /// </summary>
        public string Token { get; }

        /// <summary>
        /// Marca de tiempo de la última actividad.
        /// </summary>
        public DateTime Timestamp { get; private set; }

        /// <summary>
        /// Tiempo de vida otorgado a la sesión antes de expirar.
        /// </summary>
        public TimeSpan Life { get; }

        /// <summary>
        /// Nombre de usuario utilizado para iniciar sesión.
        /// </summary>
        public string User { get; }

        /// <summary>
        /// Inicializa una nueva instancia de la estructura
        /// <see cref="ActiveSession"/>.
        /// </summary>
        /// <param name="source">
        /// Origen de inicio de sesión para la nueva instancia.
        /// </param>
        /// <param name="token">
        /// Token de seguridad generado para esta sesión.
        /// </param>
        /// <param name="user">
        /// Nombre de usuario utilizado para iniciar sesión.
        /// </param>
        public ActiveSession(ILoginSource source, string token,string user)
        {
            Source = source;
            Token = token;
            Timestamp = DateTime.Now;
            Life = TimeSpan.FromMinutes(30);
            User = user;
        }

        /// <summary>
        /// Indica que la sesión ha consumido un servicio, por lo que aún no
        /// debe morir.
        /// </summary>
        public void Consume()
        {
            Timestamp = DateTime.Now;
        }

        /// <summary>
        /// Obtiene un valor que indica si la sesión ha expirado por
        /// inactividad.
        /// </summary>
        public bool Void => (DateTime.Now - Life) > Timestamp;
    }

#if LoginWithJsonData
#pragma warning disable IDE1006 // Estilos de nombres

    /// <summary>
    /// Describe un modelo de datos JSON para la transmisión de un nombre
    /// de usuario y una contraseña.
    /// </summary>
    public class CredInput
    {
        /// <summary>
        /// Obtiene el nombre de usuario.
        /// </summary>
        public string? user { get; set; }

        /// <summary>
        /// Obtiene la contraseña.
        /// </summary>
        public string? password { get; set; }
    }

#pragma warning restore IDE1006
#endif

    /// <summary>
    /// Controlador que gestiona los inicios de sesión en el servicio Web de
    /// Proteus.
    /// </summary>
    public class LoginController : ProteusWebController<UserService>
    {

#if DEBUG

        /// <summary>
        ///     Inicializa la clase <see cref="LoginController"/>
        /// </summary>
        static LoginController()
        {
            Sessions.Add(new ActiveSession(Proteus.Service<UserService>() ?? throw new InvalidOperationException(), "devel", "devel"));
        }

#endif

        internal static List<ActiveSession> Sessions { get; } = new List<ActiveSession>();

        /// <summary>
        /// Comprueba si el cliente solicitante tiene una sesión activa.
        /// </summary>
        /// <returns>
        /// Estado 200 si el cliente solicitante tiene una sesión activa en el
        /// sistema, estado 403 en caso contrario.
        /// </returns>
        [HttpGet("/v1/Login/CheckLogin")]
        public ActionResult<object> CheckLogin()
        {
            return !Authenticated(out var session) 
                ? StatusCode(403) 
                : (ActionResult<object>)StatusCode(200, session);
        }

        /// <summary>
        /// Enumera los orígenes de inicio de sesión disponibles en este host.
        /// </summary>
        /// <returns>
        /// Un arreglo de cadenas que contiene el nombre del tipo de cada
        /// origen de sesión disponible en este host.
        /// </returns>
        [HttpGet("/v1/Login/Sources")]
        public ActionResult<object> Sources()
        {
            return Proteus.LoginSources.Select(p=>p.GetType().Name).ToArray();
        }

#if LoginWithJsonData
        /// <summary>
        /// Ejecuta un inicio de sesión desde un cliente web.
        /// </summary>
        /// <param name="cred">
        /// Información JSON de credencial a utilizar para iniciar sesión.
        /// </param>
        /// <returns>
        /// El resultado de la operación de inicio de sesión.
        /// </returns>
        [HttpPost("/v1/login/")]
        public async Task<ActionResult> Login([FromBody]CredInput cred)
        {
            var user = cred.user;
            var password = cred.password;
#else
        /// <summary>
        /// Ejecuta un inicio de sesión desde un cliente web.
        /// </summary>
        /// <param name="user">
        /// Nombre de usuario.
        /// </param>
        /// <param name="password">
        /// Contraseña.
        /// </param>
        /// <returns>
        /// El resultado de la operación de inicio de sesión.
        /// </returns>
        [HttpPost("/v1/login/")]
        public async Task<ActionResult> Login([FromForm]string user, [FromForm]string password)
        {
#endif
            if (new[] { user, password }.IsAnyNull()) return StatusCode(400);
            foreach (var j in Proteus.LoginSources.Reverse())
            {
                var u = await j.Login(user!, password!.ToSecureString());
                switch (u.Result)
                {
                    case LoginResultCode.UnknownUser: continue;
                    case LoginResultCode.Ok:
                        var b = new byte[96];
                        using (var rng = RandomNumberGenerator.Create()) rng.GetBytes(b);
                        var token = Convert.ToBase64String(b);
                        var s = Sessions.Push(new ActiveSession(j, token, user!));
                        return StatusCode(200, new
                        {
                            id=u.Logon.StringId,
                            name = u.Logon.Name,
                            user_type = Proteus.LoginSources.ToList().IndexOf(j),
                            token = s.Token
                        });
                    case LoginResultCode.DisabledUser:
                    case LoginResultCode.InvalidPassword:
                    case LoginResultCode.InvalidToken:
                    case LoginResultCode.ExpiredToken:
                        return StatusCode(403, u.Message);

                    case LoginResultCode.NotInteractive:
                    case LoginResultCode.NotSvcUser:
                    case LoginResultCode.StationNotConfigured:
                        return StatusCode(401, u.Message);

                    default:
                        return StatusCode(400, u.Message);
                }
            }
            return StatusCode(Eegg() ? 418 : 403, LoginResultCode.UnknownUser.NameOf());
        }

        private bool Eegg()
        {
            var t = DateTime.Today;
            return t.Month == 4 && t.Day == 1;
        }
    }
}