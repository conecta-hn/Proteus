/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Plugins;
using System.Collections.Generic;
using TheXDS.Proteus.ViewModels.Base;
using TheXDS.Proteus.Widgets;
using System.Windows.Input;
using TheXDS.MCART.ViewModel;
using System.Windows;
using System.Linq;

namespace TheXDS.Proteus.ViewModels
{
    /// <summary>
    ///     ViewModel que controla la presentación visual de un módulo.
    /// </summary>
    public class ModulePageViewModel : ReportingPageHostViewModel
    {
        private UiModule _module;
        private bool _isFullMenuVisible;

        internal IPage PageInstance { get; }

        /// <summary>
        ///     Obtiene o establece el módulo gestionado por este ViewModel.
        /// </summary>
        public UiModule Module
        {
            get => _module;
            set => Change(ref _module, value);
        }

        /// <summary>
        ///     Obtiene el menú de elementos escenciales del módulo.
        /// </summary>
        public IEnumerable<Launcher> Essentials => Module.Essentials;

        /// <summary>
        ///     Obtiene el menú completo del módulo, agrupado por categorías.
        /// </summary>
        public IDictionary<string, ICollection<Launcher>> FullMenu => Module.FullMenu;

        /// <summary>
        ///     Obtiene el título de la ventana actual.
        /// </summary>
        public override string Title
        {
            get => Module.Name;
            set { }
        }

        /// <summary>
        ///     Comando de apertura de la página especial del módulo.
        /// </summary>
        public ICommand OpenModulePage { get; }

        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="ModulePageViewModel"/>.
        /// </summary>
        /// <param name="host">
        ///     Host visual de este ViewModel.
        /// </param>
        public ModulePageViewModel(IPageVisualHost host) : base(host)
        {
            RegisterPropertyChangeBroadcast(nameof(Module), new[]
            {
                nameof(Title),
                nameof(DashboardVisibility),
                nameof(ModuleDashboard),
                nameof(DashboardVisibility),
                nameof(Essentials),
                nameof(FullMenu)
            });
            RegisterPropertyChangeBroadcast(nameof(Pages),nameof(NoPageVisibility));
            
            PageInstance = host;
            OpenModulePage = new SimpleCommand(OnOpenPage);
        }

        /// <summary>
        ///     Obtiene o establece un valor que indica si el menú de al lado
        ///     está abierto o no.
        /// </summary>
        public bool IsFullMenuVisible
        {
            get => _isFullMenuVisible;
            set => Change(ref _isFullMenuVisible, value);
        }
        public Visibility DashboardVisibility => Module.ModuleDashboard is null ? Visibility.Collapsed : Visibility.Visible;

        public Visibility NoPageVisibility => Pages.Any() ? Visibility.Collapsed : Visibility.Visible;

        public UIElement ModuleDashboard => Module.ModuleDashboard;
        private void OnOpenPage()
        {
            IsFullMenuVisible = true;
            PageInstance.Activate();
        }
    }
}