/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Api;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.Models.Base;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Extensions;
using System;
using TheXDS.MCART.Types.Base;
using System.Linq.Expressions;

namespace TheXDS.Proteus.ViewModels.Base
{
    /// <summary>
    /// ViewModel que ofrece servicios de gestión y de actualización 
    /// automática de colecciones enlazadas a datos.
    /// </summary>
    /// <typeparam name="TService">
    /// Tipo de servicio principal a exponer.
    /// </typeparam>
    public abstract class ServicingPageViewModel<TService> : PageViewModel, IAsyncRefreshable where TService : Service, new()
    {
        private readonly HashSet<Func<Task>> _observables = new HashSet<Func<Task>>();

        /// <summary>
        /// Obtiene una referencia al servicio utilizado por este
        /// ViewModel.
        /// </summary>
        protected TService Service { get; } = Proteus.Service<TService>();

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="ServicingPageViewModel{TService}"/>.
        /// </summary>
        /// <param name="host">
        /// Página Host de este ViewModel.
        /// </param>
        protected ServicingPageViewModel(ICloseable host) : base(host)
        {
            RunStartup();
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="ServicingPageViewModel{TService}"/>.
        /// </summary>
        /// <param name="host">
        /// Página Host de este ViewModel.
        /// </param>
        /// <param name="closeable">
        /// Valorq ue indica si la ventana controlada por este ViewModel
        /// puede ser cerrada.
        /// </param>
        protected ServicingPageViewModel(ICloseable host, bool closeable) : base(host, closeable)
        {
            RunStartup();
        }

        /// <summary>
        /// Ejecuta una serie de operaciones de inicialización asíncrona.
        /// </summary>
        /// <returns>
        /// Una tarea que puede utilziarse para monitorear la operación 
        /// asíncrona.
        /// </returns>
        protected abstract Task OnStartup();

        /// <summary>
        /// Ejecuta las operaciones de inicialización asíncrona de este
        /// ViewModel.
        /// </summary>
        protected void RunStartup()
        {
            IsBusy = true;
            OnStartup();
            IsBusy = false;
        }

        /// <summary>
        /// Obtiene una nueva colección observable del modelo especificado,
        /// registrándola en el subsistema de actualización automática.
        /// </summary>
        /// <typeparam name="T">
        /// Modelo de la lista observable.
        /// </typeparam>
        /// <returns>
        /// Una lista observable que puede ser utilizada con total
        /// normalidad como una colección de datos.
        /// </returns>
        protected async Task<ObservableListWrap<T>> GetObservableAsync<T>() where T : ModelBase
        {
            return RegisterObservable(new ObservableListWrap<T>(await GetListAsync<T>(Service)), Service);
        }

        protected async Task<ObservableListWrap<T>> GetObservableAsync<T>(Expression<Func<T,bool>> predicate) where T : ModelBase, new()
        {
            return RegisterObservable(new ObservableListWrap<T>(await GetListAsync<T>(Service,predicate)), Service);
        }


        /// <summary>
        /// Obtiene una nueva colección observable del modelo especificado,
        /// registrándola en el subsistema de actualización automática.
        /// </summary>
        /// <typeparam name="T">
        /// Modelo de la lista observable.
        /// </typeparam>
        /// <param name="target">
        /// Objetivo de notificación de cambio en la colección.
        /// </param>
        /// <param name="props">
        /// Propiedades a notificar al ocurrir un cambio en la colección.
        /// </param>
        /// <returns>
        /// Una lista observable que puede ser utilizada con total
        /// normalidad como una colección de datos.
        /// </returns>
        protected async Task<ObservableListWrap<T>> GetObservableAsync<T>(NotifyPropertyChangeBase target, params string[] props) where T : ModelBase
        {
            var r = RegisterObservable(new ObservableListWrap<T>(await GetListAsync<T>(Service)), Service);
            r.ForwardNotify(target, props);
            return r;
        }


        /// <summary>
        /// Obtiene una nueva colección observable del modelo especificado,
        /// registrándola en el subsistema de actualización automática.
        /// </summary>
        /// <typeparam name="T">
        /// Modelo de la lista observable.
        /// </typeparam>
        /// <returns>
        /// Una lista observable que puede ser utilizada con total
        /// normalidad como una colección de datos.
        /// </returns>
        protected ObservableListWrap<T> GetObservable<T>() where T : ModelBase
        {
            return RegisterObservable(new ObservableListWrap<T>(GetList<T>(Service)), Service);
        }

        /// <summary>
        /// Obtiene una nueva colección observable del modelo especificado
        /// desde un servicio externo, registrándola en el subsistema de
        /// actualización automática.
        /// </summary>
        /// <typeparam name="T">
        /// Modelo de la lista observable.
        /// </typeparam>
        /// <returns>
        /// Una lista observable que puede ser utilizada con total
        /// normalidad como una colección de datos.
        /// </returns>
        protected async Task<ObservableListWrap<T>> GetExternalObservableAsync<T>() where T : ModelBase
        {
            var s = Proteus.Infer(typeof(T));
            return RegisterObservable(new ObservableListWrap<T>(await GetListAsync<T>(s)), s);
        }

        /// <summary>
        /// Actualiza el estado de este ViewModel.
        /// </summary>
        public override void Refresh()
        {
            RefreshAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Actualiza el estado de este ViewModel de forma asíncrona.
        /// </summary>
        /// <returns>
        /// Una tarea que puede utilziarse para monitorear la operación 
        /// asíncrona.
        /// </returns>
        public async Task RefreshAsync()
        {
            base.Refresh();
            foreach (var j in _observables) await j.Invoke();
        }
 
        private async Task LoadAsync<T>(ObservableListWrap<T> collection, Service service) where T : ModelBase
        {
            collection.Substitute(await GetListAsync<T>(service));
        }

        private async Task<IList<T>> GetListAsync<T>(Service service) where T : ModelBase
        {
            return (typeof(T).IsInstantiable()
                ? await QueryableExtensions.ToListAsync(service.All(typeof(T)))
                : service.AllBase(typeof(T)).ToList()).OfType<T>().ToList();
        }

        private async Task<IList<T>> GetListAsync<T>(Service service, Expression<Func<T,bool>> predicate) where T : ModelBase, new()
        {
            return await QueryableExtensions.ToListAsync(service.All<T>().Where(predicate));
        }

        private async Task<IList<T>> GetListBaseAsync<T>(Service service, Expression<Func<ModelBase, bool>> predicate) where T : ModelBase
        {
            return service.AllBase(typeof(T)).Where(predicate).OfType<T>().ToList();
        }







        private ObservableListWrap<T> RegisterObservable<T>(ObservableListWrap<T> observable, Service service) where T : ModelBase
        {
            _observables.Add(() => LoadAsync(observable, service));
            return observable;
        }

        private IList<T> GetList<T>(Service service) where T : ModelBase
        {
            return (typeof(T).IsInstantiable()
                ? service.All(typeof(T)).ToList()
                : service.AllBase(typeof(T)).ToList()).OfType<T>().ToList();
        }
    }
}