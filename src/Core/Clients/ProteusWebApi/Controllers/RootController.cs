/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using Microsoft.AspNetCore.Mvc;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.Controllers.Base;
using System.Linq;
using TheXDS.MCART.Component;

namespace TheXDS.Proteus.Controllers
{
    [Route("v1")]
    public class RootController : ProteusWebController
    {
        [HttpGet]
        public ActionResult Index()
        {
            return StatusCode(200, new {
                healthy = !ProteusLib.Settings.RequireNetworkServerSuccess || ProteusLib.NwClient.IsAlive,
                nwServerAlive = ProteusLib.NwClient.IsAlive,
                interactive = ProteusLib.Interactive,
                launch_config = ProteusLib.Settings,
                session = ProteusLib.Session is null ? null : new {
                    name = ProteusLib.Session.Name,
                    granted = ProteusLib.Session.DefaultGranted.ToString(),
                    revoked = ProteusLib.Session.DefaultRevoked.ToString(),
                    interactive = ProteusLib.Session.Interactive,
                    allow_multi_login = ProteusLib.Session.AllowMultiLogin
                },
                info = new AssemblyInfo(typeof(ProteusLib).Assembly),
            });
        }

        [HttpGet("Services")]
        public ActionResult Services()
        {
            var s = ProteusLib.Services;
            if (!s.Any()) return StatusCode(503);

            return StatusCode(200, s.Select(p => new
            {
                friendly_name = p.FriendlyName,
                healthy = p.IsHealthy(),
                is_elevated = p.IsElevated,
                elevation_behavior = p.ElevationBehavior.ToString(),
                uncommited_changes = p.ChangesPending(),
                info = new ExposedInfo(p),
            }));
        }

        [HttpGet("Modules")]
        public ActionResult Modules()
        {
            var s = Program._modules;
            if (!s.Any()) return StatusCode(503);

            return StatusCode(200, s.Select(p => new
            {
                controllers = p.ExploreControllers().Select(q => q.Name.Replace("Controller", string.Empty)),
                info = new ExposedInfo(p),
            }));
        }

        [HttpGet("Logcat")]
        public ActionResult Logcat()
        {
            return !(ProteusLib.MessageTarget is LogcatMessageTarget m) 
                ? StatusCode(503) 
                : (ActionResult)StatusCode(200, m.Entries.ToArray());
        }
    }
}