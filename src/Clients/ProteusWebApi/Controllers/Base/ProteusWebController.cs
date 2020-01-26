/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.Models.Base;

namespace TheXDS.Proteus.Controllers.Base
{
    /// <summary>
    ///     Clase base para un controlador web de API de Proteus que expone acceso
    ///     a un servicio de datos de Proteus.
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
        protected T Service => Proteus.Service<T>()!;

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

        /// <summary>
        /// Enumera una colección con filtros de predicado de forma paginada.
        /// </summary>
        /// <typeparam name="TModel">
        /// Tipo de elementos de la colección.
        /// </typeparam>
        /// <param name="predicates">
        /// Colección de predicados a adjuntar al Query de datos.
        /// </param>
        /// <param name="page">
        /// Número de página a obtener.
        /// </param>
        /// <param name="qty">Ítems por página.</param>
        /// <param name="totalPages">
        /// Parámetro de salida. Número total de páginas.
        /// </param>
        /// <returns>
        /// Un arreglo con los ítems de la página especificada.
        /// </returns>
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
        /// <summary>
        /// Enumera una colección de forma paginada.
        /// </summary>
        /// <typeparam name="T">Tipo de elementos de la colección.</typeparam>
        /// <param name="items">
        /// Query desde el cual obtener los ítems a enumerar.
        /// </param>
        /// <param name="page">
        /// Número de página a obtener.
        /// </param>
        /// <param name="qty">Ítems por página.</param>
        /// <param name="totalPages">
        /// Parámetro de salida. Número total de páginas.
        /// </param>
        /// <returns>
        /// Un arreglo con los ítems de la página especificada.
        /// </returns>
        protected static Task<T[]> PageUp<T>(IQueryable<T> items, int page, int qty, out int totalPages)
        {
            totalPages = (items.Count() / qty) + 1;

            if (page > 1)
            {
                items = items.Skip((page - 1) * qty);
            }
            return items.Take(qty).ToArrayAsync();
        }

        /// <summary>
        /// Obtiene una credencial que coincida con el token que forma parte de
        /// la solicitud.
        /// </summary>
        /// <returns>
        /// Un <see cref="ActiveSession"/> de la sesión correspondiente con la
        /// solicitud recibida, o <see langword="null"/> si no se ha incluido
        /// el token en la solicitud o si no existe una sesión activa con el
        /// token especificado.
        /// </returns>
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

        /// <summary>
        /// Obtiene un valor que indica si la solicitud coincide con una sesión
        /// activa.
        /// </summary>
        /// <param name="session">
        /// Parámetro de salida. Obtiene la sesión activa que ha realizado la
        /// solicitud. Si ninguna sesión activa puede ser encontrada con el
        /// token de la solicitud, o si la solicitud no contiene uno, se
        /// devolverá <see langword="default"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> si se ha encontrado una sesión activa para
        /// la solicitud, <see langword="false"/> en caso contrario.
        /// </returns>
        protected bool Authenticated(out ActiveSession session)
        {
            var a = Authenticated();
            session = a ?? default;
            return a.HasValue;
        }

        /// <summary>
        /// Obtiene un <see cref="ActionResult{TValue}"/> que devuelve todo el
        /// contenido de un archivo en el almacenamiento local del host como
        /// una cadena Base64.
        /// </summary>
        /// <param name="path">
        /// Ruta del archivo a leer dentro del almacenamiento local del host.
        /// </param>
        /// <returns></returns>
        protected ActionResult<string> FromFile(string path)
        {
            try
            {
                using var fs = new FileStream(Path.Combine(Startup.Settings.DataDir, path), FileMode.Open);
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

        /// <summary>
        /// Renderiza una imagen utilizando un método de dibujado y devuelve el
        /// resultado como una cadena Base64.
        /// </summary>
        /// <param name="renderMethod">
        /// Método que se utilizará para dibujar la imagen.
        /// </param>
        /// <param name="width">
        /// Ancho de la imagen.
        /// </param>
        /// <param name="height">
        /// Alto de la imagen.
        /// </param>
        /// <param name="format">
        /// Formato de la imagen. Si se omite, se utilizará
        /// <see cref="ImageFormat.Png"/>.
        /// </param>
        /// <returns>
        /// Una cadena Base64 con el contenido del archivo de imagen
        /// resultante.
        /// </returns>
        protected static ActionResult<string> RenderBase64(Action<Graphics> renderMethod, int width, int height, ImageFormat format)
        {
            return Convert.ToBase64String(Render(renderMethod, width, height, format).ToArray());
        }

        /// <summary>
        /// Renderiza una imagen en formato PNG utilizando un método de
        /// dibujado y devuelve el resultado como una cadena Base64.
        /// </summary>
        /// <param name="renderMethod">
        /// Método que se utilizará para dibujar la imagen.
        /// </param>
        /// <param name="width">
        /// Ancho de la imagen.
        /// </param>
        /// <param name="height">
        /// Alto de la imagen.
        /// </param>
        /// <returns>
        /// Una cadena Base64 con el contenido del archivo de imagen
        /// resultante.
        /// </returns>
        protected static ActionResult<string> RenderBase64(Action<Graphics> renderMethod, int width, int height)
        {
            return RenderBase64(renderMethod, width, height, ImageFormat.Png);
        }

        /// <summary>
        /// Renderiza una imagen utilizando un método de dibujado y devuelve el
        /// resultado como un archivo descargable.
        /// </summary>
        /// <param name="renderMethod">
        /// Método que se utilizará para dibujar la imagen.
        /// </param>
        /// <param name="width">
        /// Ancho de la imagen.
        /// </param>
        /// <param name="height">
        /// Alto de la imagen.
        /// </param>
        /// <param name="format">
        /// Formato de la imagen. Si se omite, se utilizará
        /// <see cref="ImageFormat.Png"/>.
        /// </param>
        /// <param name="mime">
        /// Metadatos de MIME que describen el tipo y formato del archivo
        /// devuelto.
        /// </param>
        /// <returns>
        /// Un archivo descargable con el contenido de imagen resultante.
        /// </returns>
        protected static FileStreamResult RenderFile(Action<Graphics> renderMethod, int width, int height, ImageFormat format, string mime)
        {
            var ms = Render(renderMethod, width, height, format);
            ms.Position = 0;
            return new FileStreamResult(ms, mime);
        }

        /// <summary>
        /// Renderiza una imagen utilizando un método de dibujado y devuelve el
        /// resultado como un archivo descargable.
        /// </summary>
        /// <param name="renderMethod">
        /// Método que se utilizará para dibujar la imagen.
        /// </param>
        /// <param name="width">
        /// Ancho de la imagen.
        /// </param>
        /// <param name="height">
        /// Alto de la imagen.
        /// </param>
        /// <param name="format">
        /// Formato de la imagen. Si se omite, se utilizará
        /// <see cref="ImageFormat.Png"/>.
        /// </param>
        /// <returns>
        /// Un archivo descargable con el contenido de imagen resultante.
        /// </returns>
        protected static FileStreamResult RenderFile(Action<Graphics> renderMethod, int width, int height, ImageFormat format)
        {
            return RenderFile(renderMethod, width, height, format, $"image/{format.ToString().ToLower()}");
        }

        /// <summary>
        /// Renderiza una imagen en formato PNG utilizando un método de
        /// dibujado y devuelve el resultado como un archivo descargable.
        /// </summary>
        /// <param name="renderMethod">
        /// Método que se utilizará para dibujar la imagen.
        /// </param>
        /// <param name="width">
        /// Ancho de la imagen.
        /// </param>
        /// <param name="height">
        /// Alto de la imagen.
        /// </param>
        /// <returns>
        /// Un archivo descargable con el contenido de imagen resultante.
        /// </returns>
        protected static FileStreamResult RenderFile(Action<Graphics> renderMethod, int width, int height)
        {
            return RenderFile(renderMethod, width, height, ImageFormat.Png, "image/png");
        }

        /// <summary>
        /// Renderiza una imagen utilizando un método de dibujado y devuelve el
        /// resultado como un <see cref="MemoryStream"/>.
        /// </summary>
        /// <param name="renderMethod">
        /// Método que se utilizará para dibujar la imagen.
        /// </param>
        /// <param name="width">
        /// Ancho de la imagen.
        /// </param>
        /// <param name="height">
        /// Alto de la imagen.
        /// </param>
        /// <param name="format">
        /// Formato de la imagen. Si se omite, se utilizará
        /// <see cref="ImageFormat.Png"/>.
        /// </param>
        /// <returns>
        /// Un <see cref="MemoryStream"/> con el contenido de imagen
        /// resultante.
        /// </returns>
        protected static MemoryStream Render(Action<Graphics> renderMethod, int width, int height, ImageFormat format)
        {
            var i = new Bitmap(width, height);
            var g = Graphics.FromImage(i);
            renderMethod(g);
            var ms = new MemoryStream();
            i.Save(ms, format);
            return ms;
        }

        /// <summary>
        /// Convierte una imagen a una cadena Base64 que puede ser devuelta por
        /// la acción de un controlador Web.
        /// </summary>
        /// <param name="i">Imagen a procesar.</param>
        /// <returns>
        /// Una cadena Base64 con el contenido de la imagen guardada en formato
        /// PNG.
        /// </returns>
        protected static ActionResult<string> PngBase64(Image i)
        {
            using var ms = new MemoryStream();
            i.Save(ms, ImageFormat.Png);
            return new ActionResult<string>(Convert.ToBase64String(ms.ToArray()));
        }

        /// <summary>
        /// Crea un archivo descargable a partir de una imagen.
        /// </summary>
        /// <param name="i">Imagen a procesar.</param>
        /// <returns>
        /// Un archivo descargable con el contenido de la imagen almacenada en
        /// formato PNG.
        /// </returns>
        protected static FileStreamResult PngFile(Image i)
        {
            var ms = new MemoryStream();
            i.Save(ms, ImageFormat.Png);
            ms.Position = 0;
            return new FileStreamResult(ms, "image/png");
        }
    }
}