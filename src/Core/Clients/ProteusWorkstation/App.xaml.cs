/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Markup;
using TheXDS.MCART.Attributes;
using TheXDS.MCART.Component;
using TheXDS.MCART.PluginSupport.Legacy;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Proteus.Component;
using TheXDS.Proteus.Config;
using TheXDS.Proteus.Plugins;
using TheXDS.Proteus.ViewModels.Base;
using TheXDS.Proteus.Widgets;
using static TheXDS.MCART.Objects;
using static TheXDS.MCART.Types.Extensions.EnumerableExtensions;

[assembly: XmlnsPrefix("http://schemas.thexds.com/2019/Proteus/page", "page")]
[assembly: XmlnsDefinition("http://schemas.thexds.com/2019/Proteus/page", "TheXDS.Proteus.Pages.Base", AssemblyName = "ProteusWorkstation")]
[assembly: XmlnsDefinition("http://schemas.thexds.com/2019/Proteus/page", "TheXDS.Proteus.Pages", AssemblyName = "ProteusWorkstation")]
[assembly: XmlnsPrefix("http://schemas.thexds.com/2019/Proteus/widgets", "Proteus")]
[assembly: XmlnsDefinition("http://schemas.thexds.com/2019/Proteus/widgets", "TheXDS.Proteus.Widgets", AssemblyName = "ProteusWorkstation")]
[assembly: XmlnsPrefix("http://schemas.thexds.com/2019/Proteus/valueconverters", "vc")]
[assembly: XmlnsDefinition("http://schemas.thexds.com/2019/Proteus/valueconverters", "TheXDS.Proteus.ValueConverters", AssemblyName = "ProteusWorkstation")]
[assembly: XmlnsDefinition("http://schemas.thexds.com/2019/Proteus/res", "TheXDS.Proteus.Resources", AssemblyName = "ProteusWorkstation")]
[assembly: Name("Proteus Workstation")]

namespace TheXDS.Proteus
{
    /// <summary>
    ///     Lógica de interacción para App.xaml
    /// </summary>
    public partial class App
    {
        private static HashSet<Tool> _tools = null!;
        internal static HashSet<UiModule> _modules = null!;
        private static HashSet<Patch> _patches = null!;
        private static HashSet<IKickStarter> _kickStarters = null!;
        private static readonly IKickStarter _fallbackKickStarter = FindFirstObject<IKickStarter>()!;

        /// <summary>
        ///     Carga los plugins de la aplicación de forma asíncrona.
        /// </summary>
        /// <returns>
        ///     Una tarea que puede utilizarse para monitorear la operación.
        /// </returns>
        public static async Task LoadPlugins()
        {
            Proteus.CommonReporter?.UpdateStatus("Cargando componentes...");
            _tools = new HashSet<Tool>(await Load<Tool>());
            _modules = new HashSet<UiModule>(await Load<UiModule>());
            _patches = new HashSet<Patch>(await Load<Patch>());
            _kickStarters = new HashSet<IKickStarter>(FindAllObjects<IKickStarter>().Prioritized());
            _kickStarters.Remove(_fallbackKickStarter);

            foreach (var j in _modules) j.AfterInitialization();
        }

        /// <summary>
        ///     Invoca una operación en el mismo hilo de la UI.
        /// </summary>
        /// <param name="action">Acción a ejecutar.</param>
        public static void UiInvoke(Action action)
        {
            Current?.Dispatcher.Invoke(action);
        }

        /// <summary>
        ///     Invoca una operación en el mismo hilo de la UI.
        /// </summary>
        /// <typeparam name="T">
        ///     Tipo de valor devuelto por la función.
        /// </typeparam>
        /// <param name="action">Función a ejecutar.</param>
        /// <returns>El resultado de la función.</returns>
        public static T UiInvoke<T>(Func<T> action)
        {
            return !(Current is null) ? Current.Dispatcher.Invoke(action) : (default);
        }

        /// <summary>
        ///     Obtiene el <see cref="IKickStarter"/> predeterminado de la aplicación.
        /// </summary>
        public static IKickStarter KickStarter
        {
            get
            {
                if (!Settings.Default.UseAltLauncher)
                {
                    return _kickStarters?.FirstOrDefault(p => p.Usable) ?? _fallbackKickStarter;
                }
                return _kickStarters.FirstOrDefault(p => p.Usable && p.GetType().Name == Settings.Default.AltLauncher) ?? _fallbackKickStarter;
            }
        }

        /// <summary>
        ///     Obtiene una página desde un <see cref="IKickStarter"/> válido.
        /// </summary>
        /// <typeparam name="TPage">
        ///     Tipo de página a obtener.
        /// </typeparam>
        /// <param name="selector">Función de selección de página.</param>
        /// <returns>
        ///     Una página obtenida desde un <see cref="IKickStarter"/> que no
        ///     haya devuelto <see langword="null"/>, o la página solicitada
        ///     integrada de la aplicación.
        /// </returns>
        public static TPage FromKickStarter<TPage>(Func<IKickStarter,TPage> selector) where TPage : IPage
        {
            foreach (var j in _kickStarters)
            {
                if (selector.Invoke(j) is TPage p) return p;
            }
            return selector.Invoke(_fallbackKickStarter);
        }

        /// <summary>
        ///     Enumera las herramientas cargadas de la aplicación.
        /// </summary>
        public static IEnumerable<Tool> Tools => _tools;

        /// <summary>
        ///     Enumera los módulos cargados de la aplicación.
        /// </summary>
        public static IEnumerable<UiModule> Modules => _modules;

        /// <summary>
        ///     Obtiene un módulo específico.
        /// </summary>
        /// <typeparam name="T">Tipo de módulo a obtener.</typeparam>
        /// <returns>
        ///     Una instancia activa del módulo del tipo especificado.
        /// </returns>
        public static T? Module<T>() where T : UiModule, new() => Modules.FirstOrDefault(p => p.GetType() == typeof(T)) as T;

        /// <summary>
        ///     Enumera los parches cargados de la aplicación.
        /// </summary>
        public static IEnumerable<Patch> Patches => _patches;

        /// <summary>
        ///     Obtiene una lista de objetos que pueden ser utilizados para
        ///     arrancar la aplicación.
        /// </summary>
        public static IEnumerable<IKickStarter> KickStarters => _kickStarters;

        private static async Task<IEnumerable<T>> Load<T>() where T : Plugin
        {
            return GetTypes<T>(true).Select(t => TypeExtensions.New<T>(t, false, Array.Empty<object>())).NotNull().Concat(await new PluginLoader().LoadEverythingAsync<T>(Settings.Default.WsPluginsDir)).Distinct(new TypeComparer<T>());
        }

        private class TypeComparer<T> : IEqualityComparer<T>
        {
            public bool Equals(T x, T y)
            {
                return x!.GetType() == y!.GetType();
            }

            public int GetHashCode(T obj)
            {
                return obj!.GetType().GetHashCode();
            }
        }

        /// <summary>
        ///     Obtiene el objeto registrado como huésped raíz de páginas para
        ///     la aplicación.
        /// </summary>
        public static IRootPageHost RootHost { get; internal set; } = null!;

        /// <summary>
        ///     Aplica los parches pertinentes a un objeto.
        /// </summary>
        /// <param name="o"></param>
        public static void ApplyPatches(object o)
        {
            foreach (var j in _patches)
            {
                if (!j.Patches(o)) continue;
                j.Apply(o);
            }
        }

        /// <summary>
        ///     Expone información básica sobre la aplicación.
        /// </summary>
        public static IExposeInfo Info { get; } = new AssemblyInfo(typeof(App).Assembly);
    }
}