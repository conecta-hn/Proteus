/*
Copyright © 2017-2020 César Andrés Morgan
Licenciado para uso interno solamente.
*/

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using TheXDS.MCART.Component;
using TheXDS.Proteus.Config;
using TheXDS.Proteus.Dialogs;
using TheXDS.Proteus.ViewModels;
using TheXDS.Proteus.ViewModels.Base;
using TheXDS.Proteus.Widgets;

namespace TheXDS.Proteus
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IPageRootVisualHost
    {
        private bool _forced = false;
        internal MainWindowViewModel Vm => Dispatcher.Invoke(() => (MainWindowViewModel)DataContext);

        /// <summary>
        /// Obtiene una referencia al host de ventanas de esta instancia.
        /// </summary>
        public IPageHost PageHost { get; set; } = null!;

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="MainWindow"/>.
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

        private void MainWindow_Closed(object? sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MainWindow_Loaded(object? sender, RoutedEventArgs e)
        {
            TryLaunchAsync();
        }

        private async void TryLaunchAsync(params Argument[] args)
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
    · Los componentes estén actualizados.
    · La aplicación haya sido configurada correctamente.
    · Se esté usando un usuario de dominio en el equipo si la conexión es a un servidor de dominio.
    · Esté conectado a la red.

Información adicional: {ex.Message}
");

                switch (Settings.Default.InitErrorAction)
                {
                    case InitErrorActions.Config:
                        TryLaunchAsync(new Cmd.ConfigureArgument());
                        break;
                    case InitErrorActions.Throw:
                        throw;
                    case InitErrorActions.Continue: break;
#if DEBUG
                    case InitErrorActions.Debug:
                        if (!Debugger.IsAttached && Debugger.Launch()) Debugger.Break();
                        break;
#endif
                    default:
                        App.UiInvoke(ForceClose);
                        break;
                }
            }
        }

        /// <summary>
        /// Activa una tab abierta en la ventana.
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
        /// Fuerza el cierre de la aplicación.
        /// </summary>
        public void ForceClose()
        {
            _forced = true;
            Application.Current.Shutdown();
        }
    }
}