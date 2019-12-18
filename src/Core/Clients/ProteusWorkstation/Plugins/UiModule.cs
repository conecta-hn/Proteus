/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Annotations;
using TheXDS.Proteus.Api;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.Pages.Base;
using TheXDS.Proteus.ViewModels.Base;
using TheXDS.Proteus.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using TheXDS.MCART;
using TheXDS.MCART.PluginSupport.Legacy;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Extensions;
using System.Threading.Tasks;

namespace TheXDS.Proteus.Plugins
{
    /// <summary>
    ///     Clase base que describe la estructura de un módulo de UI de Proteus.
    /// </summary>
    public abstract class UiModule : WpfPlugin
    {
        /// <summary>
        ///     Registra un diccionario de recursos contenido en este
        ///     ensamblado.
        /// </summary>
        /// <param name="path">
        ///     Ruta dentro del ensamblado en el cual está embebido el
        ///     diccionario.
        /// </param>
        protected void RegisterDictionary(string path)
        {
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary {Source = new Uri($@"pack://application:,,,/{Assembly.GetName().Name};component/{path}")});
        }
        
        private IPageHost _host;
        
        /// <summary>
        ///     Obtiene al host de ventana que se utilizará para contener las
        ///     ventanas abiertas de este módulo.
        /// </summary>
        public IPageHost Host
        {
            get => _host ?? App.RootHost;
            internal set => _host = value;
        }

        public ObservableCollectionWrap<Launcher> Essentials { get; }

        public ObservableDictionaryWrap<string, ICollection<Launcher>> FullMenu { get; }

        public ICollection<Launcher> CommonMenu(InteractionType type)
        {
            if (!Enum.IsDefined(typeof(InteractionType), type)) throw new ArgumentOutOfRangeException(nameof(type));
            return FullMenu[type.NameOf()];
        }

        public bool HasEssentials => Essentials.Any();

        public IStatusReporter Reporter { get; internal set; }

        public UIElement ModuleDashboard { get; protected set; }

        public UiModule()
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

        public void RegisterLauncher(Launcher launcher, string group)
        {
            if (!FullMenu.ContainsKey(group)) FullMenu.Add(group, new ObservableCollectionWrap<Launcher>(new HashSet<Launcher>()));
            FullMenu[group].Add(launcher);
        }

        public void AutoRegisterMenu<T>() where T : Service, new()
        {
            foreach (var j in Proteus.Service<T>().Models().Where(p=>!p.IsAbstract).Select(p=>p.ResolveToDefinedType()))
            {
                var descr = Crud.CrudElement.GetDescription(j);
                var type = descr?.OnModuleMenu;
                if (type is null) continue;
                void method() => Host.OpenPage(CrudPage.New(j));
                var launcher = new Launcher(descr.FriendlyName, $"Administrar {descr.FriendlyName}", method);
                RegisterLauncher(launcher, (type.Value & ~InteractionType.Essential).NameOf());
                if (type.Value.HasFlag(InteractionType.Essential)) Essentials.Add(launcher);
            }
        }

        public void ForceRegisterMenu<T>() where T : Service, new()
        {
            foreach (var j in Proteus.Service<T>().Models().Where(p => !p.IsAbstract).Select(p => p.ResolveToDefinedType()))
            {
                void method() => Host.OpenPage(CrudPage.New(j));
                var launcher = new Launcher(j.NameOf(), $"Administrar {typeof(T).NameOf()}", method);
                RegisterLauncher(launcher, InteractionType.Misc.NameOf());
                Essentials.Add(launcher);
            }
        }

        protected internal virtual void AfterInitialization() { }
    }

    /// <summary>
    ///     Clase base que describe la estructura de un módulo de UI de Proteus.
    /// </summary>
    /// <typeparam name="T">
    ///     Tipo de servicio por medio del cual este módulo interactuará con la
    ///     base de datos.
    /// </typeparam>
    public abstract class UiModule<T> : UiModule where T : Service, new()
    {
        /// <summary>
        ///     Registra de forma automática las ventanas de Crud para los
        ///     modelos administrados por el servicio <typeparamref name="T"/>.
        /// </summary>
        protected internal override void AfterInitialization()
        {
            AutoRegisterMenu<T>();
        }

        /// <summary>
        ///     Obtiene una referencia a la instancia del servicio especificado.
        /// </summary>
        protected T Service => Proteus.Service<T>();
    }
}
