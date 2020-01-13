/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TheXDS.MCART;
using TheXDS.MCART.Exceptions;
using TheXDS.MCART.Types.Base;
using TheXDS.MCART.Types.Extensions;
using TheXDS.MCART.ViewModel;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Component.Attributes;
using TheXDS.Proteus.Protocols;
using static TheXDS.MCART.Types.Extensions.MemberInfoExtensions;
using static TheXDS.MCART.Types.Extensions.StringExtensions;
using static TheXDS.MCART.Types.Extensions.TypeExtensions;

namespace TheXDS.Proteus.ViewModels.Base
{
    /// <inheritdoc />
    /// <summary>
    /// Clase base para todos los objetos que definan la capa de
    /// interacción de MVVM que además expone un servicio de forma
    /// predeterminada.
    /// </summary>
    public abstract class ProteusViewModel<T> : ProteusViewModel where T : Service, new()
    {
        /// <summary>
        /// Obtiene una referencia al servicio expuesto por este
        /// <see cref="ViewModel{T}"/>
        /// </summary>
        protected T Service => Proteus.Service<T>();

        /// <inheritdoc />
        /// <summary>
        /// Notifica de nuevos cambios existentes sobre el contenido de los
        /// campos de este <see cref="ProteusViewModel" />.
        /// </summary>
        public override void Refresh()
        {
            Service.Reload();
            base.Refresh();
        }
    }

    /// <summary>
    /// Clase base para todos los ViewModel de Proteus.
    /// </summary>
    public abstract class ProteusViewModel : ViewModelBase
    {
        static ProteusViewModel()
        {
            Proteus.NwClient.Wire(Response.ViewModelRefresh, OnViewModelRefresh);
        }

        private static void OnViewModelRefresh(Response response, BinaryReader br)
        {
            var n = br.ReadString();

            RefreshVmAsync(n).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Obtiene una instancia activa de un 
        /// <see cref="ProteusViewModel"/>.
        /// </summary>
        /// <typeparam name="T">
        /// Tipo de <see cref="ProteusViewModel"/> a obtener.
        /// </typeparam>
        /// <returns>
        /// Una instancia activa de <see cref="ProteusViewModel"/>, o
        /// <see langword="null"/> si no hay ninguna instancia activa del
        /// <see cref="ProteusViewModel"/> especificado.
        /// </returns>
        public static T Get<T>() where T: ProteusViewModel
        {
            return ActuallyActiveVms.OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// Enumera a todos los <see cref="ProteusViewModel"/> activos de 
        /// la aplicación.
        /// </summary>
        public static IEnumerable<ProteusViewModel> ActuallyActiveVms => ActiveViewModels.Select(p => p.TryGetTarget(out var q) ? q : null).NotNull().ToList();

        /// <summary>
        /// Obliga a un ViewModel con refresco asíncrono a actualizarse.
        /// </summary>
        /// <param name="typeName">
        /// Nombre del tipo de ViewModel que debe actualizarse.
        /// </param>
        /// <returns>
        /// Una tarea que pude utilizarse para observar la operación
        /// asíncrona.
        /// </returns>
        public static Task RefreshVmAsync(string typeName)
        {
            return RefreshVmAsync(p => p.GetType().ResolveToDefinedType()!.Name == typeName);
        }

        /// <summary>
        /// Obliga a todos los ViewModel con refresco asíncrono a
        /// actualizarse.
        /// </summary>
        /// <returns>
        /// Una tarea que pude utilizarse para observar la operación
        /// asíncrona.
        /// </returns>
        public static Task RefreshVmAsync() => RefreshVmAsync(_ => true);

        /// <summary>
        /// Obliga a un ViewModel con refresco asíncrono a actualizarse.
        /// </summary>
        /// <param name="predicate">
        /// Función que determina si un ViewModel se actualizará o no.
        /// </param>
        /// <returns>
        /// Una tarea que pude utilizarse para observar la operación
        /// asíncrona.
        /// </returns>
        public static async Task RefreshVmAsync(Func<ProteusViewModel, bool> predicate)
        {
            foreach (var j in ActuallyActiveVms.Where(predicate).OfType<IAsyncRefreshable>())            
                await j.RefreshAsync();
        }

        /// <summary>
        /// Conecta automáticamente todas las colecciones observables a su
        /// respectivo evento de notificación de cambios en la colección.
        /// </summary>
        protected void AutoHook()
        {
            static string UndRem(string name) => name[1].ToString().ToUpper() + name.Substring(2);

            foreach (var j in GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(p => p.FieldType.Implements<INotifyCollectionChanged>() && !p.HasAttr<NoAutoHookAttribute>()))
            {
                if (!(j.GetValue(this) is INotifyCollectionChanged c))
                {
                    c = Activator.CreateInstance(j.FieldType) as INotifyCollectionChanged
                        ?? throw new Exception($"El campo {j.Name} del modelo {GetType().Name} no fue inicializado.");
                    j.SetValue(this, c);
                }
                var name = UndRem(j.Name);
                c.CollectionChanged += (sender, e) => OnPropertyChanged(name);
            }
        }

        /// <summary>
        /// Indica a una colección que debe notificar del cambio de un
        /// campo cuando la misma cambie.
        /// </summary>
        /// <param name="collection">Colección a conectar.</param>
        /// <param name="fieldName">Campo a notificar.</param>
        protected void Hook(INotifyCollectionChanged collection, string fieldName)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (fieldName.IsEmpty()) throw new ArgumentNullException(nameof(fieldName));
            collection.CollectionChanged += (sender, e) => OnPropertyChanged(fieldName);
        }

        /// <summary>
        /// Indica a una colección que debe notificar del cambio de un
        /// conjunto de campos cuando la misma cambie.
        /// </summary>
        /// <param name="collection">Colección a conectar.</param>
        /// <param name="fields">Campos a notificar.</param>
        protected void Hook(INotifyCollectionChanged collection, params string[] fields)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (fields is null) throw new ArgumentNullException(nameof(fields));
            if (!fields.Any()) throw new EmptyCollectionException(fields);
            if (!fields.Any(StringExtensions.IsEmpty))
                throw new ArgumentNullException(nameof(fields));

            collection.CollectionChanged += (sender, e) =>
            {
                foreach (var j in fields) OnPropertyChanged(j);
            };
        }

        /// <summary>
        /// Indica a una colección que debe notificar del cambio de un
        /// conjunto de campos cuando la misma cambie.
        /// </summary>
        /// <param name="collection">Colección a conectar.</param>
        /// <param name="fields">Campos a notificar.</param>
        protected void Hook(INotifyCollectionChanged collection, IEnumerable<string> fields)
        {
            Hook(collection, fields.ToArray());
        }
        
        /// <summary>
        /// Obtiene el tipo real del objeto, de-referenciando un tipo
        /// dinámico en caso de ser necesario.
        /// </summary>
        public Type ActualType => GetActualType(GetType());

        private Type GetActualType(Type? t)
        {
            if (t is null) return typeof(object);
            return t.Module.Assembly.IsDynamic ? GetActualType(t.BaseType) : t;
        }

        /// <summary>
        /// Obtiene un nombre amigable para este elemento.
        /// </summary>
        public string ModelFriendlyName => ActualType.NameOf();

        /// <summary>
        /// Obtiene una lista con las referencias débiles de todos los
        /// <see cref="ViewModels"/> activos en la aplicación.
        /// </summary>
        public static HashSet<WeakReference<ProteusViewModel>> ActiveViewModels { get; } = new HashSet<WeakReference<ProteusViewModel>>();

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="ViewModels"/>.
        /// </summary>
        protected ProteusViewModel()
        {
            lock (ActiveViewModels)
            {
                ActiveViewModels.Add(new WeakReference<ProteusViewModel>(this));
            }
        }

        /// <summary>
        /// Ejecuta una acción, colocando a este <see cref="ViewModels"/> en
        /// un estado de espera.
        /// </summary>
        /// <param name="action">
        /// Acción a ejecutar. Puede ser un método asíncrono.
        /// </param>
        /// <param name="commands">
        /// Comandos a deshabilitar/reactivar.
        /// </param>
        protected void Perform(Action action, params SimpleCommand[] commands)
        {
            var states = SaveStates(commands);
            try { action?.Invoke(); }
            finally { RestoreStates(states); }
        }

        private void RestoreStates(Dictionary<SimpleCommand, bool> states)
        {
            foreach (var j in states)            
                j.Key.SetCanExecute(j.Value);
            
            IsBusy = false;
        }

        private Dictionary<SimpleCommand,bool> SaveStates(IEnumerable<SimpleCommand> commands)
        {
            var states = new Dictionary<SimpleCommand, bool>();
            foreach (var j in commands)
            {
                states.Add(j, j.CanExecute(null));
                j.SetCanExecute(false);
            }
            IsBusy = true;
            return states;
        }

        /// <summary>
        /// Ejecuta una tarea, colocando a este <see cref="ViewModels"/> en
        /// un estado de espera.
        /// </summary>
        /// <param name="task">Tarea a ejecutar.</param>
        /// <param name="commands">
        /// Comandos a deshabilitar/reactivar.
        /// </param>
        protected Task PerformAsync(Func<Task> task, params SimpleCommand[] commands)
        {
            var states = SaveStates(commands);
            try { return task?.Invoke() ?? Task.CompletedTask; }
            finally { RestoreStates(states); }
        }

        /// <summary>
        /// Ejecuta una tarea, colocando a este <see cref="ViewModels"/> en
        /// un estado de espera.
        /// </summary>
        /// <param name="task">Tarea a ejecutar.</param>
        /// <param name="commands">
        /// Comandos a deshabilitar/reactivar.
        /// </param>
        /// <returns>
        /// El resultado de la operación asíncrona.
        /// </returns>
        protected Task<DetailedResult> PerformAsync(Func<Task<DetailedResult>> task, params SimpleCommand[] commands)
        {
            var states = SaveStates(commands);
            try { return task?.Invoke() ?? Task.FromResult(DetailedResult.Ok); }
            finally { RestoreStates(states); }
        }

        /// <summary>
        /// Ejecuta acciones adicionales para asegurar la destrucción de este ViewModel.
        /// </summary>
        public void Destroy()
        {
            lock (ActiveViewModels)
            {
                var vm = ActiveViewModels.FirstOrDefault(p => p.TryGetTarget(out var m) && m == this);
                ActiveViewModels.Remove(vm);
            }
        }

        /// <summary>
        /// Destruye esta instancia de la clase <see cref="ProteusViewModel"/>.
        /// </summary>
        ~ProteusViewModel()
        {
            Destroy();
        }
    }
}