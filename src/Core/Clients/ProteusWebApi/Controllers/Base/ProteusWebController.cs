/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Component;
using Newtonsoft.Json;
using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using TheXDS.MCART.Resources;
using System.Drawing.Imaging;
using TheXDS.Proteus.Models.Base;
using System.Linq.Expressions;

namespace TheXDS.Proteus.Controllers.Base
{
    /// <summary>
    ///     Clase base para un controlador web de API de Proteus que expone acceso
    ///     a un servicio de datos de ProteusLib.
    /// </summary>
    /// <typeparam name="T">
    ///     Tipo de servicio expuesto.
    /// </typeparam>
    [Route("v1/[controller]")]
    public abstract class ProteusWebController<T> : ProteusWebController where T : Service
    {
        private static readonly JsonSerializerSettings _noParent = new JsonSerializerSettings()
        {
            ContractResolver = new NoParentingContractResolver()
        };
        private static readonly JsonSerializerSettings _noChildren = new JsonSerializerSettings()
        {
            ContractResolver = new NotResolvedRelationsContractResolver()
        };

        /// <summary>
        ///     Obtiene una referencia al servicio asociado a este controlador
        ///     de API.
        /// </summary>
        protected T Service => ProteusLib.Service<T>();

        /// <summary>
        ///     Muestra el estado de salud del controlador.
        /// </summary>
        /// <returns>
        ///     Estado Http 200 si el servicio está activo y slaudable.
        ///     Estado http 503 si el servicio no está disponible.
        /// </returns>
        [HttpGet("/v1/[controller]")]
        [HttpGet("/v1/[controller]/Health")]
        public async Task<ActionResult> Health()
        {
            if (Service is null) return StatusCode(503);
            return StatusCode(200, new
            {
                name = Service.FriendlyName,
                version = Service.Version,
                healthy = await Service.IsHealthyAsync(),
                is_elevated = Service.IsElevated,
                elevation_behavior = Service.ElevationBehavior.ToString(),
                uncommited_changes = Service.ChangesPending()
            });
        }

        /// <summary>
        ///     Lee directamente una entidad de la base de datos.
        /// </summary>
        /// <param name="model">Modelo a obtener.</param>
        /// <param name="id">Id del modelo a obtener.</param>
        /// <returns>
        ///     Un <see cref="JsonResult"/> con la entidad encontrada.
        /// </returns>
        [HttpGet("/v1/[controller]/db/{model}/{id}")]
        public async Task<ActionResult> ReadDb(string model, string id)
        {
            if (!Service.Hosts(model, out var tModel)) return StatusCode(400);
            var obj = await Service.GetAsync(tModel, id);
            if (obj is null) return StatusCode(404);
            return new JsonResult(obj, _noParent);
        }

        /// <summary>
        ///     Lee una tabla completa de la base de datos.
        /// </summary>
        /// <param name="model">Tabla de modelos a obtener.</param>
        /// <param name="page">Número de página a obtener.</param>
        /// <param name="itemsPerPage">
        ///     Cantidad de elementos por página a devolver.
        /// </param>
        /// <returns>
        ///     Un <see cref="JsonResult"/> con la tabla solicitada.
        /// </returns>
        [HttpGet("/v1/[controller]/db/{model}")]
        public async Task<ActionResult> PagerDb(string model, [FromQuery] int page = 1, [FromQuery] int itemsPerPage = 10)
        {
            if (!Service.Hosts(model, out var tModel) || page < 1 || itemsPerPage < 1) return StatusCode(400);
            IQueryable<ModelBase> obj = Service.All(tModel).OrderBy(p => p.Id);
            var totalPages = (obj.Count() / itemsPerPage) + 1;

            if (page > 1)
            {
                obj = obj.Skip((page - 1) * itemsPerPage);
            }

            return new JsonResult(
                new
                {
                    currentPage = page,
                    lastPage = totalPages,
                    data = await obj.Take(itemsPerPage).ToArrayAsync()
                }, _noChildren);
        }
        protected Task<TModel[]> PageUp<TModel>(int page, int qty, out int totalPages, params Expression<Func<TModel, bool>>[] predicates) where TModel : ModelBase, new()
        {
            IQueryable<TModel> q = Service.All<TModel>().OrderBy(p => p.Id);

            foreach (var j in predicates)
            {
                q = q.Where(j);
            }

            return PageUp(q, page, qty, out totalPages);
        }
    }


    /// <summary>
    ///     Clase base para un controlador web de API de Proteus.
    /// </summary>
    [Route("v1/[controller]/[action]")]
    [ApiController]
    public abstract class ProteusWebController : ControllerBase
    {
        protected Task<T[]> PageUp<T>(IQueryable<T> items, int page, int qty, out int totalPages)
        {
            totalPages = (items.Count() / qty) + 1;

            if (page > 1)
            {
                items = items.Skip((page - 1) * qty);
            }
            return items.Take(qty).ToArrayAsync();
        }



        protected ActiveSession? Authenticated()
        {
            if (!Request.Headers.ContainsKey("token")) return null;            
            var token = Request.Headers["token"].FirstOrDefault();
            ActiveSession? found = null;

            Parallel.ForEach(LoginController.Sessions, (s, t) => 
            {
                if (s.Token == token)
                {                    
                    found = s;
                    s.Consume();
                    t.Stop();
                }
            });

            return found;
        }

        protected bool Authenticated(out ActiveSession session)
        {
            var a = Authenticated();
            session = a ?? default;
            return a.HasValue;
        }

        protected ActionResult<string> FromFile(string path)
        {
            try
            {
                using var fs = new FileStream($"{Startup.Settings.DataDir}\\{path}", FileMode.Open);
                using var br = new BinaryReader(fs);
                return Convert.ToBase64String(br.ReadBytes((int)fs.Length));
            }
            catch (FileNotFoundException fex)
            {
                return StatusCode(404, fex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        protected ActionResult<string> RenderBase64(Action<Graphics> renderMethod,int width, int height, ImageFormat format)
        {
            return Convert.ToBase64String(Render(renderMethod, width, height, format).ToArray());
        }

        protected ActionResult<string> RenderBase64(Action<Graphics> renderMethod, int width, int height)
        {
            return RenderBase64(renderMethod, width, height, ImageFormat.Png);
        }

        protected FileStreamResult RenderFile(Action<Graphics> renderMethod, int width, int height, ImageFormat format, string mime)
        {
            var ms = Render(renderMethod, width, height, format);
            ms.Position = 0;
            return new FileStreamResult(ms, mime);
        }

        protected FileStreamResult RenderFile(Action<Graphics> renderMethod, int width, int height)
        {
            return RenderFile(renderMethod, width, height, ImageFormat.Png, "image/png");
        }

        protected MemoryStream Render(Action<Graphics> renderMethod, int width, int height, ImageFormat format)
        {
            var i = new Bitmap(width, height);
            var g = Graphics.FromImage(i);
            renderMethod(g);
            var ms = new MemoryStream();
            i.Save(ms, format);
            return ms;
        }
        protected ActionResult<string> PngBase64(Image i)
        {
            using var ms = new MemoryStream();
            i.Save(ms, ImageFormat.Png);
            return new ActionResult<string>(Convert.ToBase64String(ms.ToArray()));
        }
        protected FileStreamResult PngFile(Image i)
        {
            var ms = new MemoryStream();
            i.Save(ms, ImageFormat.Png);
            ms.Position = 0;
            return new FileStreamResult(ms, "image/png");
        }
    }
}