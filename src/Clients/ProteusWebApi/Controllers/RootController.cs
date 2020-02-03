/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using Microsoft.AspNetCore.Mvc;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.Controllers.Base;
using System.Linq;
using TheXDS.MCART.Component;

namespace TheXDS.Proteus.Controllers
{
    /// <summary>
    /// Controlador raíz de la aplicación. Provee de datos de telemetría
    /// básicos y estado de salud del servicio Web y diversos componentes de
    /// Proteus.
    /// </summary>
    [Route("v1")]
    public class RootController : ProteusWebController
    {
        /// <summary>
        /// Obtiene información básica de telemetría del servicio de API Web.
        /// </summary>
        /// <returns>
        /// La información básica de telemetría del servicio de API Web.
        /// </returns>
        [HttpGet]
        public ActionResult Index()
        {
            return StatusCode(200, new {
                healthy = !Proteus.Settings?.RequireNetworkServerSuccess ?? false || Proteus.NwClient.IsAlive,
                nwServerAlive = Proteus.NwClient.IsAlive,
                interactive = Proteus.Interactive,
                launch_config = Proteus.Settings,
                session = Proteus.Session is null ? null : new {
                    name = Proteus.Session.Name,
                    granted = Proteus.Session.DefaultGranted.ToString(),
                    revoked = Proteus.Session.DefaultRevoked.ToString(),
                    interactive = Proteus.Session.Interactive,
                    allow_multi_login = Proteus.Session.AllowMultiLogin
                },
            });
        }

        /// <summary>
        /// Obtiene información sobre los servicios cargados.
        /// </summary>
        /// <returns>
        /// Un <see cref="ActionResult"/> que obtiene una bloque de datos en
        /// formato JSON.
        /// </returns>
        [HttpGet("Services")]
        public ActionResult Services()
        {
            var s = Proteus.Services;
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

        /// <summary>
        /// Obtiene información sobre los módulos web cargados.
        /// </summary>
        /// <returns>
        /// Un <see cref="ActionResult"/> que obtiene una bloque de datos en
        /// formato JSON.
        /// </returns>
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

        /// <summary>
        /// Obtiene una copia de los mensajes generados por Proteus.
        /// </summary>
        /// <returns>
        /// Un <see cref="ActionResult"/> que obtiene una bloque de datos en
        /// formato JSON.
        /// </returns>
        [HttpGet("Logcat")]
        public ActionResult Logcat()
        {
            return !(Proteus.MessageTarget is LogcatMessageTarget m) 
                ? StatusCode(503) 
                : (ActionResult)StatusCode(200, m.Entries.ToArray());
        }
    }
}