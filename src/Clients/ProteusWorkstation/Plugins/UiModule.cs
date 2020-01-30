/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TheXDS.MCART;
using TheXDS.MCART.PluginSupport.Legacy;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.Pages.Base;
using TheXDS.Proteus.ViewModels.Base;
using TheXDS.Proteus.Widgets;

namespace TheXDS.Proteus.Plugins
{
    /// <summary>
    /// Clase base que describe la estructura de un módulo de UI de Proteus.
    /// </summary>
    public abstract class UiModule : WpfPlugin
    {
        /// <summary>
        /// Registra un diccionario de recursos contenido en este
        /// ensamblado.
        /// </summary>
        /// <param name="path">
        /// Ruta dentro del ensamblado en el cual está embebido el
        /// diccionario.
        /// </param>
        protected void RegisterDictionary(string path)
        {
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary {Source = new Uri($@"pack://application:,,,/{Assembly.GetName().Name};component/{path}")});
        }
        
        private IPageHost? _host;
        
        /// <summary>
        /// Obtiene al host de ventana que se utilizará para contener las
        /// ventanas abiertas de este módulo.
        /// </summary>
        public IPageHost Host
        {
            get => _host ?? App.RootHost;
            internal set => _host = value;
        }

        /// <summary>
        /// Enumera los <see cref="Launcher"/> que han sido marcados como
        /// escenciales.
        /// </summary>
        public ObservableCollectionWrap<Launcher> Essentials { get; }

        /// <summary>
        /// Enumera todos los <see cref="Launcher"/> disponibles en este
        /// módulo de forma agrupada.
        /// </summary>
        public ObservableDictionaryWrap<string, ICollection<Launcher>> FullMenu { get; }

        /// <summary>
        /// Obtiene los <see cref="Launcher"/> asociados a un tipo de
        /// interacción en particular.
        /// </summary>
        /// <param name="type">Tipo de interacción a buscar.</param>
        /// <returns>
        /// Una colección de todos los <see cref="Launcher"/> que son del tipo
        /// de interacción especificado.
        /// </returns>
        public ICollection<Launcher> CommonMenu(InteractionType type)
        {
            if (!Enum.IsDefined(typeof(InteractionType), type)) throw new ArgumentOutOfRangeException(nameof(type));
            return FullMenu[type.NameOf()];
        }

        /// <summary>
        /// Obtiene un valor que indica si el módulo incluye botones de acceso 
        /// escenciales.
        /// </summary>
        public bool HasEssentials => Essentials.Any();

        /// <summary>
        /// Obtiene una referencia al <see cref="IStatusReporter"/> asociado a 
        /// este módulo.
        /// </summary>
        public IStatusReporter? Reporter { get; internal set; }

        /// <summary>
        /// Obtiene una referencia al objeto de UI que describe la página de
        /// Dashboard del módulo.
        /// </summary>
        public UIElement? ModuleDashboard { get; protected set; }

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="UiModule"/>.
        /// </summary>
        protected UiModule()
        {
            Essentials = new ObservableCollectionWrap<Launcher>(new HashSet<Launcher>(PluginInteractions.Where(p => p.Action.Method.HasAttr<EssentialAttribute>()).Select(p => (Launcher)p)));

            FullMenu = new ObservableDictionaryWrap<string, ICollection<Launcher>>(new Dictionary<string, ICollection<Launcher>>());
            LoadMenu();
        }

        private void LoadMenu()
        {
            FullMenu.Clear();
            foreach (var j in PluginInteractions)
            {

                var g =
                    j.Action.Method.GetAttr<InteractionGroupAttribute>()?.Value.OrNull() ??
                    j.Action.Method.GetAttr<InteractionTypeAttribute>()?.Value.NameOf() ??
                    InteractionType.Misc.NameOf();
                RegisterLauncher(j, g);
            }
        }

        /// <summary>
        /// Registra un <see cref="Launcher"/> en el grupo especificado.
        /// </summary>
        /// <param name="launcher"><see cref="Launcher"/> a registrar.</param>
        /// <param name="group">
        /// Grupo en el cual registrar el <paramref name="launcher"/>.
        /// </param>
        public void RegisterLauncher(Launcher launcher, string group)
        {
            if (!FullMenu.ContainsKey(group)) FullMenu.Add(group, new ObservableCollectionWrap<Launcher>(new HashSet<Launcher>()));
            FullMenu[group].Add(launcher);
        }

        /// <summary>
        /// Registra un <see cref="Launcher"/> en el grupo especificado.
        /// </summary>
        /// <param name="launcher"><see cref="Launcher"/> a registrar.</param>
        /// <param name="type">
        /// Tipo de interacción en la cual registrar el 
        /// <paramref name="launcher"/>.
        /// </param>
        public void RegisterLauncher(Launcher launcher, InteractionType type)
        {
            RegisterLauncher(launcher, type.NameOf());
        }

        /// <summary>
        /// Registra automáticamente todos los modelos de datos en los que la
        /// descripción de los mismos contenga una llamada al método
        /// <see cref="Crud.Base.CrudDescriptor{T}.OnModuleMenu()"/> o de una 
        /// de sus sobrecargas.
        /// </summary>
        /// <typeparam name="T">
        /// Tipo de servicio desde el cual obtener una lista de modelos.
        /// </typeparam>
        public void AutoRegisterMenu<T>() where T : Service, new()
        {
            foreach (var j in Proteus.Service<T>()!.Models().Where(p=>!p.IsAbstract).Select(p=>p.ResolveToDefinedType()!))
            {
                var descr = Crud.CrudElement.GetDescription(j);
                var type = descr?.OnModuleMenu;
                if (type is null) continue;

                void method()
                {
                    if (descr?.LaunchPreCondition?.Invoke() ?? true)
                        Host.OpenPage(CrudPage.New(j));
                }

                var launcher = new Launcher(descr?.FriendlyName ?? j.Name, $"Administrar {descr?.FriendlyName ?? j.Name}", method);
                RegisterLauncher(launcher, (type.Value & ~InteractionType.Essential).NameOf());
                if (type.Value.HasFlag(InteractionType.Essential)) Essentials.Add(launcher);
            }
        }

        /// <summary>
        /// Fuerza el registro de todos los modelos de datos encontrados en el
        /// servicio del tipo especificado.
        /// </summary>
        /// <typeparam name="T">
        /// Tipo de servicio desde el cual obtener una lista de modelos.
        /// </typeparam>
        public void ForceRegisterMenu<T>() where T : Service, new()
        {
            foreach (var j in Proteus.Service<T>()!.Models().Where(p => !p.IsAbstract).Select(p => p.ResolveToDefinedType()!))
            {
                var descr = Crud.CrudElement.GetDescription(j);
                void method()
                {
                    if (descr?.LaunchPreCondition?.Invoke() ?? true)
                        Host.OpenPage(CrudPage.New(j));
                }

                var launcher = new Launcher(descr?.FriendlyName ?? j.Name, $"Administrar {descr?.FriendlyName ?? j.Name}", method);
                RegisterLauncher(launcher, InteractionType.Misc.NameOf());
                Essentials.Add(launcher);
            }
        }

        /// <summary>
        /// Método invalidable que ofrece un lugar para ejecutar acciones
        /// posteriores a la inicialización del módulo.
        /// </summary>
        protected internal virtual void AfterInitialization() { }
    }

    /// <summary>
    /// Clase base que describe la estructura de un módulo de UI de Proteus.
    /// </summary>
    /// <typeparam name="T">
    /// Tipo de servicio por medio del cual este módulo interactuará con la
    /// base de datos.
    /// </typeparam>
    public abstract class UiModule<T> : UiModule where T : Service, new()
    {
        /// <summary>
        /// Registra de forma automática las ventanas de Crud para los
        /// modelos administrados por el servicio <typeparamref name="T"/>.
        /// </summary>
        protected internal override void AfterInitialization()
        {
            AutoRegisterMenu<T>();
        }

        /// <summary>
        /// Obtiene una referencia a la instancia del servicio especificado.
        /// </summary>
        protected T? Service => Proteus.Service<T>();
    }
}
