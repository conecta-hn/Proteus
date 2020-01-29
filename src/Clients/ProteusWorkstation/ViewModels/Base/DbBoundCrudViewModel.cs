/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Api;
using TheXDS.Proteus.Models.Base;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace TheXDS.Proteus.ViewModels.Base
{
    /// <summary>
    /// ViewModel que gestiona una ventana de Crud enlazada directamente a
    /// una base de datos.
    /// </summary>
    public class DbBoundCrudViewModel : CrudCollectionViewModelBase
    {
        private readonly IQueryable<ModelBase>? _source;

        private static IQueryable<ModelBase>? Infer(Type model)
        {
            return Proteus.InferService(model)?.All(model) ?? Proteus.InferBaseService(model)?.AllBase(model);
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="DbBoundCrudViewModel"/>.
        /// </summary>
        /// <param name="model">Modelo único de datos.</param>
        public DbBoundCrudViewModel(Type model) : base(TryGet(model), new[] { model })
        {
            _source = Infer(model);
        }

        private static ICollection<ModelBase> TryGet(Type model)
        {
            var tries = 0;
            while (true)
            {
                try
                {
                    return Infer(model).ToList();
                }
                catch
                {
                    if (tries++ == 10) throw;
                }
            }
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="DbBoundCrudViewModel"/>.
        /// </summary>
        /// <param name="source">Origen de datos a utilizar.</param>
        /// <param name="models">Modelos asociados de datos.</param>
        public DbBoundCrudViewModel(IQueryable<ModelBase> source, params Type[] models) : base(source.ToList(), models)
        {
            _source = source;
        }

        /// <summary>
        /// Ejecuta la operación de guardado sobre la base de datos.
        /// </summary>
        /// <param name="entity">Entidad a guardar.</param>
        /// <returns></returns>
        protected override async Task<DetailedResult> PerformSave(ModelBase entity)
        {
            var t = await PerformAsync(() => NewMode ? Service!.AddAsync(entity) : Service!.SaveAsync());
            if (t.Result == Result.Ok)
            {
                /* HACK: se debe restablecer de manera independiente el
                 * elemento seleccionado porque al actualizar una lista, los
                 * controles desechan el objeto actualmente seleccionado.*/
                var tmp = Selection;
                Source.Substitute(await Query());
                Selection = tmp;
            }
            return t;
        }

        /// <summary>
        /// Ejecuta una operación de eliminado de información de la
        /// colección activa.
        /// </summary>
        /// <param name="o">
        /// Elemento a eliminar.
        /// </param>
        protected override async void OnDelete(object? o)
        {
            if (!(o is ModelBase m)) return;
            var t = await PerformAsync(() => Service!.PurgeAsync(m));
            if (t.Result == Result.Ok) Source.Substitute(await Query());
        }

        private async Task<List<ModelBase>> Query()
        {
            if (_source is IDbAsyncEnumerable<ModelBase>)
            {
                Service!.Reload(_source);
                return await _source.ToListAsync();
            }
            else
            {
                return _source.ToList();
            }            
        }
    }
}