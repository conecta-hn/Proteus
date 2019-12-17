/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using Microsoft.AspNetCore.Mvc;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Controllers.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Proteus.Controllers
{
    public struct ActiveSession
    {
        public ILoginSource Source { get; }
        public string Token { get; }
        public DateTime Timestamp { get; private set; }
        public TimeSpan Life { get; }
        public string User { get; }
        public ActiveSession(ILoginSource source, string token,string user)
        {
            Source = source;
            Token = token;
            Timestamp = DateTime.Now;
            Life = TimeSpan.FromMinutes(30);
            User = user;
        }

        public void Consume()
        {
            Timestamp = DateTime.Now;
        }
        public bool Void => (DateTime.Now - Life) > Timestamp;
    }
    public class LoginController : ProteusWebController<UserService>
    {
        public class CredInput
        {
            public string user { get; set; }
            public string password { get; set; }
        }

#if DEBUG
        /// <summary>
        ///     Inicializa la clase <see cref="LoginController"/>
        /// </summary>
        static LoginController()
        {
            Sessions.Add(new ActiveSession(ProteusLib.Service<UserService>(), "devel", "devel"));
        }
#endif

        internal static List<ActiveSession> Sessions { get; } = new List<ActiveSession>();

        [HttpGet("/v1/Login/CheckLogin")]
        public ActionResult<object> CheckLogin()
        {
            return !Authenticated(out var session) 
                ? StatusCode(403) 
                : (ActionResult<object>)StatusCode(200, session);
        }

        [HttpGet("/v1/Login/Sources")]
        public ActionResult<object> Sources()
        {
            return ProteusLib.LoginSources.Select(p=>p.GetType().Name).ToArray();
        }

        [HttpPost("/v1/login/")]
        //public async Task<ActionResult> Login([FromForm]string user, [FromForm]string password)
        public async Task<ActionResult> Login([FromBody]CredInput cred)
        {
            foreach (var j in ProteusLib.LoginSources.Reverse())
            {
                var u = await j.Login(cred.user, cred.password.ToSecureString());
                switch (u.Result)
                {
                    case LoginResultCode.UnknownUser: continue;
                    case LoginResultCode.Ok:
                        var b = new byte[96];
                        using (var rng = RandomNumberGenerator.Create()) rng.GetBytes(b);
                        var token = Convert.ToBase64String(b);
                        var s = Sessions.Push(new ActiveSession(j, token, cred.user));
                        return StatusCode(200, new
                        {
                            id=u.Logon.StringId,
                            name = u.Logon.Name,
                            user_type = ProteusLib.LoginSources.ToList().IndexOf(j),
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