/*
Copyright © 2017-2019 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using TheXDS.Proteus.Config;
using TheXDS.Proteus.Dialogs;
using TheXDS.Proteus.ViewModels;
using TheXDS.Proteus.ViewModels.Base;
using TheXDS.Proteus.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace TheXDS.Proteus
{
    /// <summary>
    ///     Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IPageRootVisualHost
    {
        private bool _forced = false;
        internal MainWindowViewModel Vm => Dispatcher.Invoke(() => (MainWindowViewModel)DataContext);

        /// <summary>
        ///     Obtiene una referencia al host de ventanas de esta instancia.
        /// </summary>
        public IPageHost PageHost { get; set; } = null!;

        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="MainWindow"/>.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            DataContext = new MainWindowViewModel(this);
            Closing += MainWindow_Closing;
            Closed += MainWindow_Closed;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_forced)
            {
                if (Settings.Default.ConfirmExit)
                    e.Cancel = !MessageSplash.Ask("Salir", "¿Está seguro que desea salir?");
                if (e.Cancel) return;
            }
            e.Cancel = false;
            PageHost?.Pages.Clear();
            Proteus.Logout(false);
        }

        private void MainWindow_Closed(object? sender, EventArgs e) => Application.Current.Shutdown();

        /// <summary>
        ///     Realiza tareas de inicialización una vez abierta la ventana.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Loaded(object? sender, RoutedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(_ => OnLoaded()));
        }

        private async void OnLoaded()
        {
            try
            {
                await Vm.LaunchAsync();
            }
            catch (Exception ex)
            {
                Proteus.MessageTarget?.Stop($@"Ha ocurrido un problema iniciando {App.Info.Name}.

Asegúrese de que:

    · La aplicación incluya todos los componentes.
    · La aplicación haya sido configurada correctamente.
    · Se esté usando un usuario de dominio en el equipo si la conexión es a un servidor de dominio.
    · Esté conectado a la red.

Información adicional: {ex.Message}
");
                App.UiInvoke(ForceClose);
            }
        }

        /// <summary>
        ///     Activa una tab abierta en la ventana.
        /// </summary>
        /// <param name="page"></param>
        public void Activate(IPage page)
        {
            TabHost.SelectedItem = page;
        }

        void IPage.Activate()
        {
            BringIntoView();
        }

        /// <summary>
        ///     Fuerza el cierre de la aplicación.
        /// </summary>
        public void ForceClose()
        {
            _forced = true;
            Close();
        }
    } 
}